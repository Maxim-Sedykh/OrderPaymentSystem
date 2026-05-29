<div align="center">

# OrderPaymentSystem

**Система управления заказами, платежами и товарами**

RESTful API на ASP.NET Core с чистой архитектурой, богатой доменной моделью и production-ready инфраструктурой

[![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet)](https://dot.net)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15-4169E1?logo=postgresql)](https://www.postgresql.org/)
[![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?logo=docker)](https://www.docker.com/)
[![Redis](https://img.shields.io/badge/Redis-Cache-DC382D?logo=redis)](https://redis.io/)

</div>

---

## Содержание

- [О проекте](#о-проекте)
- [Архитектура](#архитектура)
- [Структура решения](#структура-решения)
- [Технологический стек](#технологический-стек)
- [Бизнес-логика](#бизнес-логика)
- [API Endpoints](#api-endpoints)
- [Инфраструктура и DevOps](#инфраструктура-и-devops)
- [Тестирование](#тестирование)
- [Быстрый старт](#быстрый-старт)

---

## О проекте

OrderPaymentSystem — pet-проект, демонстрирующий инженерный подход к разработке backend-систем на .NET. Реализован полный цикл работы с заказами: от добавления товаров в корзину до оплаты и отслеживания статуса доставки.

Проект построен по принципам **Clean Architecture** с элементами **DDD** (богатые доменные модели, инкапсуляция бизнес-правил в сущностях). Каждый слой имеет чёткую зону ответственности, а архитектурные границы проверяются автоматически тестами.

<img width="1538" height="998" alt="556155010-a23868f9-9090-4edd-b16c-5cfc348394b4" src="https://github.com/user-attachments/assets/9ffb7308-8e73-4392-97be-053a1cc2a409" />
Рисунок 1. Swagger

---

## Архитектура

```
┌─────────────────────────────────────────────────────┐
│                    Presentation                     │
│                  OrderPaymentSystem.Api             │
│        (Controllers, Middleware, Swagger, JWT)      │
└──────────────────────┬──────────────────────────────┘
                       │
          ┌────────────┴────────────┐
          ▼                         ▼
┌──────────────────┐   ┌──────────────────────────────┐
│   Application    │   │      Infrastructure          │
│  (Services,      │◄──│   OrderPaymentSystem.DAL     │
│   Specifications,│   │  (Repositories, EF Context,  │
│   DTOs,          │   │   Redis, Migrations)         │
│   Validations)   │   └──────────────────────────────┘
└────────┬─────────┘
         │
         ▼
┌──────────────────┐   ┌──────────────────┐
│     Domain       │   │      Shared      │
│  (Entities,      │◄──│  (BaseResult,    │
│   Interfaces)    │   │   Exceptions)    │
└──────────────────┘   └──────────────────┘
```

**Правило зависимостей:** внешний слой зависит от внутреннего, но не наоборот. Domain не имеет зависимостей от других слоёв.

---

## Структура решения

```
OrderPaymentSystem/
├── src/
│   ├── Core/
│   │   ├── OrderPaymentSystem.Domain/          # Сущности, интерфейсы репозиториев
│   │   ├── OrderPaymentSystem.Application/     # Сервисы, DTO, спецификации, валидация
│   │   └── OrderPaymentSystem.Shared/          # Базовые классы, Result-паттерн
│   ├── Infrastructure/
│   │   └── OrderPaymentSystem.DAL/             # EF Core, репозитории, Redis-кэш, миграции
│   └── Presentation/
│       └── OrderPaymentSystem.Api/             # Controllers, middleware, конфигурация
├── tests/
│   ├── UnitTests/                              # Юнит-тесты бизнес-логики
│   ├── Integration/                            # Интеграционные тесты (TestContainers)
│   └── Architecture/                           # Архитектурные тесты (NetArchTest)
├── benchmarks/
│   └── OrderPaymentSystem.Benchmarks/          # Бенчмарки (BenchmarkDotNet)
├── deploy/
│   ├── docker-compose.yml                      # Полный стек: API, PostgreSQL, Redis, ELK, Prometheus, Grafana
│   ├── prometheus.yml                          # Конфигурация Prometheus
│   └── .env.template                           # Шаблон переменных окружения
└── docs/                                       # Документация
```

---

## Технологический стек

### Ядро

| Технология | Назначение |
|---|---|
| **.NET 10** | Платформа |
| **ASP.NET Core Web API** | RESTful API |
| **Entity Framework Core** | ORM, работа с БД через репозитории |
| **PostgreSQL** | Основная БД |
| **Mapster** | Маппинг DTO ↔ сущностей |
| **FluentValidation** | Валидация входных моделей |

### Безопасность и аутентификация

| Технология | Назначение |
|---|---|
| **JWT Bearer** | Аутентификация и авторизация |
| **Role-based access** | Ролевая модель (Admin / User) |
| **Refresh-токены** | Продление сессии без повторного логина |

### Инфраструктура

| Технология | Назначение |
|---|---|
| **Redis** | Кэширование (MessagePack-сериализация) |
| **Polly** | Resilience: retry, circuit breaker, timeout |
| **Hangfire** | Фоновые задачи (очистка токенов, отмена просроченных заказов) |
| **ELK Stack** | Централизованное логирование (Serilog → Elasticsearch → Kibana) |
| **Prometheus + Grafana** | Сбор и визуализация метрик |
| **Health Checks** | Мониторинг состояния всех сервисов |

### DevOps и качество кода

| Технология | Назначение |
|---|---|
| **Docker Compose** | Контейнеризация полного стека |
| **Central Package Management** | Централизованное управление версиями NuGet |
| **Nullable reference types** | Строгая null-безопасность |
| **TreatWarningsAsErrors** | Предупреждения = ошибки компиляции |
| **EnforceCodeStyleInBuild** | Стиль кода проверяется при сборке |
| **API Versioning** | Версионирование API |

### Тестирование

| Технология | Назначение |
|---|---|
| **xUnit + Moq + FluentAssertions** | Юнит-тесты |
| **TestContainers** | Интеграционные тесты с реальными БД и Redis |
| **NetArchTest** | Автоматическая проверка архитектурных правил |
| **BenchmarkDotNet** | Профилирование производительности |

---

## Бизнес-логика

### Доменные сущности

| Сущность | Поведение |
|---|---|
| **User** | Создание, смена пароля, управление ролями |
| **Product** | CRUD, изменение цены, списание остатков |
| **BasketItem** | Управление корзиной, проверка наличия на складе |
| **Order** | Создание, подтверждение, отгрузка, пересчёт суммы, управление позициями |
| **OrderItem** | Добавление/удаление/обновление количества |
| **Payment** | Создание, обработка оплаты, расчёт сдачи |
| **Role / UserRole** | Управление ролями пользователей |
| **UserToken** | Refresh-токены с автоматической очисткой просроченных |

### Паттерны

- **Repository + Unit of Work** — абстракция доступа к данным
- **Specification** — переиспользуемые запросы (фильтрация, includes, no-tracking)
- **Result** — единый формат ответов бизнес-логики без исключений
- **Builder** — построение тестовых данных
- **Rich Domain Model** — бизнес-правила инкапсулированы внутри сущностей

### Фоновые задачи (Hangfire)

- **Ежедневно** — автоматическая отмена просроченных заказов в статусе Pending
- **Еженедельно** — очистка истёкших refresh-токенов

---

## API Endpoints

| Метод | Endpoint | Описание | Доступ |
|---|---|---|---|
| `POST` | `/api/v1/auth/register` | Регистрация | Все |
| `POST` | `/api/v1/auth/login` | Авторизация | Все |
| `POST` | `/api/v1/auth/refresh` | Обновление токена | Все |
| `GET` | `/api/v1/products` | Список товаров | Все |
| `POST` | `/api/v1/products` | Создание товара | Admin |
| `PUT` | `/api/v1/products/{id}` | Обновление товара | Admin |
| `DELETE` | `/api/v1/products/{id}` | Удаление товара | Admin |
| `GET` | `/api/v1/basket` | Корзина пользователя | User |
| `POST` | `/api/v1/basket` | Добавить в корзину | User |
| `PATCH` | `/api/v1/basket/{id}` | Изменить количество | User |
| `DELETE` | `/api/v1/basket/{id}` | Удалить из корзины | User |
| `POST` | `/api/v1/orders` | Создать заказ | User |
| `GET` | `/api/v1/orders` | Заказы пользователя | User |
| `PATCH` | `/api/v1/orders/{id}/status` | Обновить статус | User |
| `POST` | `/api/v1/orders/{id}/ship` | Отправить заказ | User |
| `POST` | `/api/v1/payments` | Создать платёж | User |
| `POST` | `/api/v1/payments/{id}/complete` | Завершить платёж | User |
| `GET` | `/api/v1/roles` | Список ролей | Admin |
| `POST` | `/api/v1/roles` | Создать роль | Admin |
| `POST` | `/api/v1/users/{userId}/roles` | Назначить роль | Admin |

Полная интерактивная документация доступна через **Swagger UI** после запуска проекта.

---

## Инфраструктура и DevOps

### Docker Compose (полный стек)

```
┌─────────────┐  ┌─────────────┐  ┌─────────────┐
│ API (:5000) │  │  PostgreSQL │  │   Redis     │
│ + Swagger   │  │   (:5432)   │  │  (:6379)    │
└──────┬──────┘  └─────────────┘  └─────────────┘
       │
       ├──────────────┬──────────────────┐
       ▼              ▼                  ▼
┌─────────────┐ ┌─────────────┐  ┌─────────────┐
│ ELK Stack   │ │ Prometheus  │  │  Grafana    │
│(:9200/:5601)│ │  (:9090)    │  │  (:3000)    │
└─────────────┘ └─────────────┘  └─────────────┘
       │
       ▼
┌─────────────┐
│  pgAdmin    │
│  (:5050)    │
└─────────────┘
```

### Observability

- **Serilog** → структурированные логи → **Elasticsearch** → **Kibana** (поиск инцидентов)
- **Prometheus** → метрики API (request rate, latency, errors) → **Grafana** (дашборды)
- **Health Checks** — мониторинг PostgreSQL, Redis, Elasticsearch через `/health`

### Resilience

- **Polly** — retry-политики, circuit breaker, timeout для внешних вызовов
- **Rate Limiting** — защита от перегрузки API
- **Redis-кэш** — снижает нагрузку на БД, MessagePack для компактной сериализации

---

## Тестирование

### Три уровня тестов

```
tests/
├── UnitTests/              # Бизнес-логика: сервисы, сущности, маппинги
│                           # Moq для мокирования, FluentAssertions для asserts
├── Integration/            # Полный цикл: Controllers → DB → Redis
│                           # TestContainers (PostgreSQL, Redis, Elasticsearch)
└── Architecture/           # Автоматическая проверка чистой архитектуры
                            # NetArchTest: зависимости, именование, расположение
```

### Архитектурные тесты проверяют

- Dependency rule: внешние слои не зависят от внутренних
- Repository-реализации находятся только в DAL
- Сервисы имеют `internal` доступ
- Корректные namespace и именование

---

## Быстрый старт

### Предварительные требования

- [.NET 10 SDK](https://dot.net)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

### Запуск через Docker Compose

```bash
# 1. Клонируйте репозиторий
git clone https://github.com/Maxim-Sedykh/OrderPaymentSystem.git
cd OrderPaymentSystem

# 2. Создайте файл .env в папке deploy/ (шаблон — .env.template)
cd deploy
cp .env.template .env
# Заполните переменные: POSTGRES_PASSWORD, JWT_KEY, ADMIN_LOGIN, ADMIN_PASSWORD

# 3. Запустите стек
docker-compose up -d

# 4. Откройте Swagger
# https://localhost:5001/swagger
```

### Запуск локально (без Docker)

```bash
# 1. Настройте user-secrets
dotnet user-secrets set "ConnectionStrings:PostgresSQL" "Server=localhost;Port=5432;Database=OrderPaymentSystem;Username=postgres;Password=postgresql"
dotnet user-secrets set "JwtSettings:JwtKey" "ваш-секретный-ключ-минимум-32-символа"
dotnet user-secrets set "RedisSettings:Url" "localhost:6379"
dotnet user-secrets set "AdminSettings:Login" "admin"
dotnet user-secrets set "AdminSettings:Password" "admin123"
dotnet user-secrets set "ElasticConfiguration:Uri" "http://localhost:9200"

# 2. Запустите PostgreSQL и Redis (через Docker или локально)

# 3. Примените миграции и запустите
cd src/Presentation/OrderPaymentSystem.Api
dotnet ef database update --project ../../Infrastructure/OrderPaymentSystem.DAL
dotnet run
```

### Первый запрос

```bash
# Авторизация
curl -X POST https://localhost:5001/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"login": "admin", "password": "ваш-пароль"}'

# Ответ содержит AccessToken — используйте его в Authorization: Bearer <token>
```

---

<div align="center">

**Автор:** [Максим Седых](https://t.me/maximka_se)

</div>
