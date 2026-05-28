# ADR-0002: Использование MessagePack для сериализации кэша

## Статус

Принят

## Контекст

Проект OrderPaymentSystem использует Redis как распределённый кэш-слой для улучшения производительности. При кэшировании данных нам нужно сериализовать объекты в бинарный формат для хранения в Redis и десериализовать обратно при получении из кэша.

Сериализация по умолчанию в .NET's `IDistributedCache` основана на JSON (используя `System.Text.Json` или `Newtonsoft.Json`), что имеет несколько ограничений для high-performance сценариев:

### Проблема с JSON сериализацией

1. **Производительность**: JSON сериализация/десериализация относительно медленная из-за парсинга строк и рефлексии
2. **Размер**: JSON многословен и создаёт бо́льшие размеры payload
3. **Аллокация**: Строковая сериализация создаёт больше давления на garbage collector
4. **Типы**: JSON требует метаданные типа для правильной полиморфной сериализации

### Результаты бенчмарков

Из `ProductServiceBenchmarks` с 10,000 продуктами:

| Метод | Среднее | Ошибка | СтдОткл | Аллокировано | Аллок. Ratio |
|--------|-------:|-------:|--------:|-------------:|-------------:|
| С Redis Cache (MessagePack) | 19.13 ms | 0.537 ms | 1.515 ms | 2.67 MB | 0.72 |
| Без Cache (DB запрос) | 20.75 ms | 0.757 ms | 2.136 ms | 3.7 MB | 1.00 |

Результаты показывают:
- **На 8% быстрее** с MessagePack кэшем
- **На 28% меньше** аллокации памяти
- Значительно снижено давление на GC

## Решение

Мы принимаем **MessagePack** как формат сериализации для операций Redis кэша.

### Реализация

```csharp
public sealed class RedisCacheService : ICacheService
{
    private readonly MessagePackSerializerOptions _options;

    public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger)
    {
        var resolver = CompositeResolver.Create(
            ContractlessStandardResolver.Instance,
            StandardResolver.Instance
        );

        _options = MessagePackSerializerOptions.Standard
            .WithResolver(resolver);

        MessagePackSerializer.DefaultOptions = _options;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct)
    {
        var data = await _cache.GetAsync(key, ct);
        return data is null || data.Length == 0
            ? null
            : MessagePackSerializer.Deserialize<T>(data, _options, ct);
    }

    public async Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions? options, CancellationToken ct)
    {
        var data = MessagePackSerializer.Serialize(value, cancellationToken: ct);
        await _cache.SetAsync(key, data, options ?? GetDefaultCacheOptions(), ct);
    }
}
```

### Конфигурация

- **Resolver**: `ContractlessStandardResolver` для гибкости без атрибутов
- **Compression**: Бинарный формат MessagePack по умолчанию компактный
- **Совместимость**: Использует `CompositeResolver` для максимальной совместимости

## Преимущества

### Производительность
- Бинарный формат быстрее сериализуется/десериализуется
- Нет оверхеда на парсинг строк
- Оптимизирован для общих .NET типов

### Размер
- 30-50% меньше payload по сравнению с JSON
- Сниженный сетевой перенос в/из Redis
- Сниженное использование памяти Redis

### Типобезопасность
- Сохраняет информацию о .NET типах
- Лучшая обработка сложных типов
- Поддержка полиморфизма

### Developer Experience
- Прямая замена для JSON сериализации
- Не нужны изменения кода в кэшируемых DTOs
- Автоматическая обработка сложных object graph

## Последствия

### Позитивные

1. **Улучшенная производительность кэша**: Более быстрая сериализация/десериализация
2. **Сниженное использование памяти**: Меньшие размеры payload
3. **Лучшая масштабируемость**: Сниженное давление на GC при высокой нагрузке
4. **Прозрачность**: Работает с существующей абстракцией `IDistributedCache`

### Негативные

1. **Бинарный формат**: Не читаем человеком как JSON (сложнее отладка)
2. **Зависимость**: Добавляет зависимость от пакета MessagePack
3. **Версионирование**: Изменения схемы требуют инвалидации кэша
4. **Кривая обучения**: Команда не знакомая с MessagePack

### Смягчение рисков

- Оставить JSON логирование для отладки проблем кэша
- Версионировать ключи кэша при изменении DTOs: `products:v1`, `products:v2`
- Документировать использование MessagePack в developer guide
- Использовать `ContractlessStandardResolver` чтобы избежать атрибутов на DTOs

## Альтернативы

### JSON (System.Text.Json)
- ✅ Читаемый человеком, лёгкая отладка
- ✅ Встроен в .NET
- ❌ Медленнее производительность
- ❌ Большие размеры payload

### JSON (Newtonsoft.Json)
- ✅ Зрелый, battle-tested
- ✅ Больше функций чем System.Text.Json
- ❌ Медленнее чем MessagePack
- ❌ Большие размеры payload

### Protobuf
- ✅ Быстрый и компактный
- ✅ Язык-независимый
- ❌ Требует .proto файлы или атрибуты
- ❌ Более сложная настройка
- ❌ Overkill для .NET-only кэша

### BinaryFormatter (Устаревший)
- ❌ **Уязвимости безопасности**
- ❌ Обозначен Microsoft как устаревший
- ❌ Не кроссплатформенный

## Ссылки

- [MessagePack Official Documentation](https://msgpack.org/)
- [MessagePack for C#](https://github.com/MessagePack-CSharp/MessagePack-CSharp)
- [Redis Caching Best Practices](https://redis.io/docs/manual/patterns/caching/)
- [Benchmark Results](benchmarks/OrderPaymentSystem.Benchmarks/bin/Release/net10.0/BenchmarkDotNet.Artifacts/results/)

## Связанные решения

- [ADR-0001: Использование Clean Architecture](0001-use-clean-architecture.md)
