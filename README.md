# Система платежей и заказов

Этот проект представляет собой систему управления платежами и заказами, разработанную с использованием ASP.NET Web API.
Проект предоставляет API для создания, обновления и отслеживания платежей и заказов.

## Особенности

- Проект на новом .NET 10
- Бизнес-логика построена на управлении заказами, товарами и платежами
- Просмотр заказов пользователя с помощью его корзины.
- Отслеживание статуса заказов и платежей.
- Аутентификация пользователей для доступа к функционалу системы.
- Проект создан с использованием чистой архитектуры и частью DDD в виде богатых доменных моделей.
- Помимо чистой архитектуры в проекте используются паттерны - Репозиторий, Спецификация, UnitOfWork, Builder

## Технологии

- ASP.NET Web API для создания RESTful API.
- Entity Framework для работы с базой данных.
- Mapster для маппинга объектов.
- JWT Bearer для аутентификации пользователей.
- PostgreSQL для хранения данных.
- Swagger для отправки http-запросов
- ELK для ведения логов.
- FluentValidation для валидации моделей.
- Docker для контейнеризации
- Redis для кэширования
- xunit и Moq для юнит-тестов
- BenchmarkDotNet для бенчмаркинга
- NetArchTest для архитектурных тестов
- TestContainers для интеграционных тестов
- HealthChecks для основных сервисов
- Hangfire для фоновых задач
- MessagePack для оптимизированной сериализации данных из Redis
- Prometheus и Graphana для сбора метрик

<img width="1538" height="998" alt="image" src="https://github.com/user-attachments/assets/a23868f9-9090-4edd-b16c-5cfc348394b4" />

## Установка и запуск

1. Клонируйте репозиторий на свой локальный компьютер.
2. Установите необходимые зависимости с помощью NuGet Package Manager.
3. Настройте строку подключения в пользовательских секретах. Пример секретов 
{
  "RedisSettings:Url": "localhost:6379",
  "Kestrel:Certificates:Development:Password": "e65619f5-0635-49e5-abeb-d25380a83bb8",
  "JwtSettings:JwtKey": "7f3a5b8c2d1e4f9a0b6c3d8e5f2a1b4c7d9e0f2b5a8c1d4e7f9b0a3c6d2e5f8c",
  "ConnectionStrings:PostgresSQL": "Server=localhost;Port=5432;Database=OrderPaymentSystem;Username=postgres;Password=postgresql",
  "AdminSettings:Login": "admin",
  "AdminSettings:Password": "12345",
  "ElasticConfiguration:Uri": "http://localhost:9200"
}
4. Перейдите в терминале в папку deploy и выполните команду docker-compose up -d
5. Для запуска контейнера API - нужно добавить в папку deploy/.env конфиги, вместо localhost нужно использовать названия сервисов из docker-compose. Пример файла .env указан в соседнем файле .env.template
6. Обратитесь к документации API, которая доступна по адресу https://localhost:7281/index.html, если запускате API контейнер, то https://localhost:5001/index.html

## Пример использования

Авторизуйтесь от лица админа. Креды админа будут тем, что вы пропишете в AdminSettings:Login и AdminSettings:Password.

Контроллер Auth, метод Login. auth/login
{
  "login": "admin",
  "password": "12345"
}

Скопируйте AccessToken из результата и авторизуйтесь с помощью окна авторизации

<img src="https://github.com/Maxim-Sedykh/OrderPaymentSystem/assets/125740808/f210685b-2aa3-431a-9fda-e9aec838f6f7" width=33%>
<img width="33%" height="456" alt="image" src="https://github.com/user-attachments/assets/f88a8e29-3dc2-4623-a8fe-7f2fe9b21e54" />
<img src="https://github.com/Maxim-Sedykh/OrderPaymentSystem/assets/125740808/c108f201-8636-418d-b181-80d670e9e946" width=33%>

Теперь вы можете использовать все API Методы и использовать весь функционал!




