# ADR-0003: Реализация паттернов Resilience с Polly

## Статус

Предложен

## Контекст

OrderPaymentSystem — это распределённая система с несколькими внешними зависимостями:

- **PostgreSQL database**
- **Redis cache**
- **Elasticsearch** для логирования
- **Внешние API** (потенциальная будущая интеграция)

В распределённых системах отказы неизбежны:
- Сети падают или замедляются
- Сервисы становятся временно недоступны
- Базы данных испытывают проблемы с подключением
- Превышаются rate limits

### Текущее состояние

В настоящее время приложение имеет минимальный resilience:
- Базовая обработка исключений через `ExceptionHandlingMiddleware`
- Result pattern для корректного распространения ошибок
- Health checks для мониторинга

**Проблема:** Временный сбой в Redis или БД приводит к неудаче всего запроса, что ухудшает пользовательский опыт.

## Решение

Мы принимаем **Polly** для реализации паттернов resilience во всём приложении.

### Паттерны для реализации

#### 1. Retry Pattern

Для временных сбоев (сетевые проблемы, временная недоступность):

```csharp
var retryStrategy = new ResiliencePipelineBuilder<HttpResponseMessage>()
    .AddRetry(new RetryStrategyOptions<HttpResponseMessage>
    {
        ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
            .Handle<HttpRequestException>()
            .HandleResult(response => response.StatusCode == HttpStatusCode.ServiceUnavailable),
        BackoffType = DelayBackoffType.Exponential,
        UseJitter = true,
        MaxRetryAttempts = 3,
        Delay = TimeSpan.FromSeconds(1)
    })
    .Build();
```

#### 2. Circuit Breaker Pattern

Для предотвращения каскадных сбоёв когда зависимость стабильно падает:

```csharp
var circuitBreaker = new ResiliencePipelineBuilder()
    .AddCircuitBreaker(new CircuitBreakerStrategyOptions
    {
        ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
            .Handle<HttpRequestException>()
            .HandleResult(response => (int)response.StatusCode >= 500),
        FailureRatio = 0.5,
        SamplingDuration = TimeSpan.FromSeconds(30),
        MinimumThroughput = 10,
        BreakDuration = TimeSpan.FromSeconds(30),
        OnOpened = args =>
        {
            _logger.LogWarning("Circuit breaker открыт для {Operation}", args.OperationName);
            return ValueTask.CompletedTask;
        }
    })
    .Build();
```

#### 3. Timeout Pattern

Для предотвращения зависающих запросов:

```csharp
var timeout = new ResiliencePipelineBuilder()
    .AddTimeout(TimeSpan.FromSeconds(30))
    .Build();
```

#### 4. Fallback Pattern

Для предоставления ухудшенной функциональности когда основная зависимость падает:

```csharp
var cacheFallback = new ResiliencePipelineBuilder<T?>()
    .AddFallback(new FallbackStrategyOptions<T>
    {
        ShouldHandle = new PredicateBuilder<T>()
            .Handle<RedisException>(),
        FallbackAction = args => Outcome.FromResult(default(T))
    })
    .Build();
```

### Стратегия реализации

#### Resilience для Базы данных

```csharp
// В DbContext или Repository
public class ResilientDbContext : ApplicationDbContext
{
    private readonly ResiliencePipeline _pipeline;

    public override async Task<int> SaveChangesAsync(CancellationToken ct)
    {
        return await _pipeline.ExecuteAsync(async ct =>
            await base.SaveChangesAsync(ct), ct);
    }
}
```

#### Resilience для Redis Cache

```csharp
public class ResilientCacheService : ICacheService
{
    private readonly ICacheService _inner;
    private readonly ResiliencePipeline _pipeline;

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct)
    {
        return await _pipeline.ExecuteAsync(async ct =>
            await _inner.GetAsync<T>(key, ct), ct);
    }
}
```

#### Resilience для HTTP Client

```csharp
services.AddHttpClient("ExternalApi")
    .AddResilienceHandler("pipeline", builder =>
    {
        builder
            .AddRetry(/* ... */)
            .AddCircuitBreaker(/* ... */)
            .AddTimeout(TimeSpan.FromSeconds(30));
    });
```

## Преимущества

1. **Улучшенная надёжность**: Временные сбои не приводят к отказам запросов
2. **Лучший UX**: Graceful degradation вместо ошибок
3. **Предотвращение каскадных сбоев**: Circuit breaker защищает downstream сервисы
4. **Производительность**: Retry логика обрабатывает временные сетевые проблемы
5. **Observability**: Встроенные метрики для мониторинга resilience

## Последствия

### Позитивные

1. Обрабатывает реальные сбои распределённых систем
2. Стандартизированные паттерны resilience по всему приложению
3. Настраиваемый для каждой зависимости
4. Богатая observability с телеметрией Polly

### Негативные

1. **Сложность**: Больше движущих частей для понимания и поддержки
2. **Задержка**: Retries добавляют задержку (смягчается exponential backoff)
3. **Тестирование**: Сложнее протестировать все сценарии сбоев
4. **Конфигурация**: Требуется настройка для каждой зависимости

### Стоимость реализации

- **Усилия**: 2-3 недели для полной реализации
- **Обучение**: Команде нужно понять паттерны Polly
- **Тестирование**: Дополнительные unit/integration тесты нужны

## Альтернативы

### Ручная Retry логика
- ❌ Изобретаем велосипед
- ❌ Несогласованные реализации
- ❌ Сложно тестировать
- ❌ Нет observability

### Istio/Linkerd (Service Mesh)
- ✅ Resilience на уровне инфраструктуры
- ✅ Без изменений кода
- ❌ Требует Kubernetes
- ❌ Более сложная инфраструктура
- ❌ Overkill для текущей настройки

### Встроенные Retries от Microsoft (ограниченные)
- ✅ Часть .NET
- ❌ Менее гибкие чем Polly
- ❌ Меньше поддерживаемых паттернов

## План реализации

### Фаза 1: Core Resilience (1 неделя)
1. Добавить Polly пакеты
2. Определить resilience стратегии для:
   - Подключений к БД
   - Redis cache
3. Unit тесты для каждой стратегии

### Фаза 2: HTTP Resilience (3 дня)
1. Добавить resilience к внешним HTTP вызовам
2. Реализовать circuit breakers
3. Добавить timeouts

### Фаза 3: Observability (3 дня)
1. Добавить телеметрию Polly
2. Создать дашборды для состояния circuit breaker
3. Алерты при чрезмерных retry

### Фаза 4: Тестирование (1 неделя)
1. Chaos engineering тесты
2. Fault injection тестирование
3. Load testing с отказами

## Пример конфигурации

```json
{
  "Resilience": {
    "Database": {
      "RetryCount": 3,
      "RetryDelay": "00:00:01",
      "CircuitBreakerFailureThreshold": 0.5,
      "CircuitBreakerDuration": "00:00:30"
    },
    "Redis": {
      "RetryCount": 2,
      "RetryDelay": "00:00:00.5",
      "Timeout": "00:00:05"
    },
    "ExternalApi": {
      "RetryCount": 3,
      "CircuitBreakerSamplingDuration": "00:00:30",
      "Timeout": "00:00:30"
    }
  }
}
```

## Ссылки

- [Polly Documentation](https://www.pollyproject.org/)
- [Microsoft Resilience Project](https://www.microsoft.com/en-us/research/project/project-polly-resilience-framework-for-net/)
- [Retry Pattern (Microsoft)](https://docs.microsoft.com/en-us/azure/architecture/patterns/retry)
- [Circuit Breaker Pattern (Microsoft)](https://docs.microsoft.com/en-us/azure/architecture/patterns/circuit-breaker)

## Связанные решения

- [ADR-0001: Использование Clean Architecture](0001-use-clean-architecture.md)
- [ADR-0002: Использование MessagePack для сериализации кэша](0002-messagepack-for-cache-serialization.md)

## Открытые вопросы

1. Использовать Polly v7 или v8? (Рекомендация: v8 с новыми API)
2. Нужно ли chaos engineering тестирование? (Рекомендация: Да, Фаза 4)
3. Должно ли состояние circuit breaker персиститься? (Рекомендация: In-memory изначально)
