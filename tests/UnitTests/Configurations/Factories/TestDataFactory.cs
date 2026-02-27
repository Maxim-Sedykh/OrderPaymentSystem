using OrderPaymentSystem.Application.DTOs.Auth;
using OrderPaymentSystem.Application.DTOs.Basket;
using OrderPaymentSystem.Application.DTOs.Payment;
using OrderPaymentSystem.Application.DTOs.Product;
using OrderPaymentSystem.UnitTests.Configurations.Factories.Builders;

namespace OrderPaymentSystem.UnitTests.Configurations.Factories;

/// <summary>
/// Сборник фабрик для построения моковых сущностей
/// </summary>
public static class TestDataFactory
{
    /// <summary>
    /// Билдер для создания пользователя
    /// </summary>
    public static UserBuilder User => new();

    /// <summary>
    /// Билдер для создания элемента корзины
    /// </summary>
    public static BasketItemBuilder BasketItem => new();

    /// <summary>
    /// Билдер для создания заказа
    /// </summary>
    public static OrderBuilder Order => new();

    /// <summary>
    /// Билдер для создания элемента заказа
    /// </summary>
    public static OrderItemBuilder OrderItem => new();

    /// <summary>
    /// Билдер для создания товара
    /// </summary>
    public static ProductBuilder Product => new();

    /// <summary>
    /// Билдер для создания платежа
    /// </summary>
    public static PaymentBuilder Payment => new();

    /// <summary>
    /// Билдер для создания роли
    /// </summary>
    public static RoleBuilder Role => new();

    /// <summary>
    /// Создать моковый <see cref="ProductDto"/>
    /// </summary>
    public static ProductDto CreateProductDto(int id = 1, string name = "Laptop")
        => new(id, name, "Description", 1000m, 10, DateTime.UtcNow);

    /// <summary>
    /// Создать моковый <see cref="LoginUserDto"/>
    /// </summary>
    public static LoginUserDto CreateLoginDto(string login = "testuser", string password = "password123")
        => new(login, password);

    /// <summary>
    /// Создать моковый <see cref="RegisterUserDto"/>
    /// </summary>
    public static RegisterUserDto CreateRegisterDto(string login = "newuser")
        => new(login, "password123", "password123");

    /// <summary>
    /// Создать моковый <see cref="PaymentDto"/>
    /// </summary>
    public static PaymentDto CreatePaymentDto(string login = "newuser")
        => new()
        {
            Id = 1,
            AmountPayed = 50m,
            AmountToPay = 50m,
            OrderId = 1,
            CashChange = 5m
        };

    /// <summary>
    /// Создать моковый <see cref="BasketItemDto"/>
    /// </summary>
    public static BasketItemDto CreateBasketItemDto(long id = 0, Guid? userId = null, int productId = 0, int quantity = 0)
        => new(id, userId ?? Guid.NewGuid(), productId, quantity);
}
