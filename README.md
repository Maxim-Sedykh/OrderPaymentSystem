# Система платежей и заказов

Этот проект представляет собой систему управления платежами и заказами, разработанную с использованием ASP.NET Web API.
Проект предоставляет API для создания, обновления и отслеживания платежей и заказов.

## Особенности

- Создание новых заказов с указанием деталей заказа.
- Просмотр заказов пользователя с помощью его корзины.
- Отслеживание статуса заказов и платежей.
- Аутентификация пользователей для доступа к функционалу системы.
- Проект создан с использованием чистой архитектуры.

## Технологии

- ASP.NET Web API для создания RESTful API.
- Entity Framework для работы с базой данных.
- AutoMapper для маппинга объектов.
- JWT Bearer для аутентификации пользователей.
- PostgreSQL для хранения данных.
- Serilog для ведения логов.
- FluentValidation для валидации моделей.
- Docker для контейнеризации
- RabbitMQ для брокерства сообщений
- Redis для кэширования
- xunit и Moq для юнит-тестов
- Prometheus и Graphana для метрик
- MediatR для реализации паттерна CQRS

<img src="https://github.com/Maxim-Sedykh/OrderPaymentSystem/assets/125740808/0b3b1f34-43cf-4fa0-8179-23736b5da701" width=100%>

## Установка и запуск

1. Клонируйте репозиторий на свой локальный компьютер.
2. Установите необходимые зависимости с помощью NuGet Package Manager.
3. Создайте базу данных в SQL Server и настройте строку подключения в пользовательских секретах.
4. Запустите проект в Visual Studio или из командной строки => dotnet run
5. Обратитесь к документации API, которая доступна по адресу https://localhost:44348/index.html

## Пример использования

Авторизуйтесь от лица админа

Контроллер Auth, метод Login

{
  "login": "Maximlog",
  "password": "1234567"
}

Скопируйте AccessToken из результата и авторизуйтесь с помощью окна авторизации

<img src="https://github.com/Maxim-Sedykh/OrderPaymentSystem/assets/125740808/f210685b-2aa3-431a-9fda-e9aec838f6f7" width=33%>
<img src="https://github.com/Maxim-Sedykh/OrderPaymentSystem/assets/125740808/3f496b19-cdfb-4bb3-891b-ba1cadff841f" width=33%>
<img src="https://github.com/Maxim-Sedykh/OrderPaymentSystem/assets/125740808/c108f201-8636-418d-b181-80d670e9e946" width=33%>

Теперь вы можете использовать все API Методы и использовать весь функционал!




