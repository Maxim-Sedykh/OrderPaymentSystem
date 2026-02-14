using OrderPaymentSystem.Application.DTOs.Auth;
using OrderPaymentSystem.Application.DTOs.Basket;
using OrderPaymentSystem.Application.DTOs.Payment;
using OrderPaymentSystem.Application.DTOs.Product;
using OrderPaymentSystem.UnitTests.Configurations.Factories.Builders;

namespace OrderPaymentSystem.UnitTests.Configurations.Factories;

public static class TestDataFactory
{
    public static UserBuilder User => new();
    public static UserTokenBuilder UserToken => new();

    public static LoginUserDto CreateLoginDto(string login = "testuser", string password = "password123")
        => new(login, password);

    public static RegisterUserDto CreateRegisterDto(string login = "newuser")
        => new(login, "password123", "password123");

    public static PaymentDto CreatePaymentDto(string login = "newuser")
        => new()
        {
            Id = 1,
            AmountPayed = 50m,
            AmountToPay = 50m,
            OrderId = 1,
            CashChange = 5m
        };

    public static BasketItemDto CreateBasketItemDto(long id = 0, Guid? userId = null, int productId = 0, int quantity = 0)
        => new(id, userId ?? Guid.NewGuid(), productId, quantity);

    public static BasketItemBuilder BasketItem => new();

    public static OrderBuilder Order => new();
    public static OrderItemBuilder OrderItem => new();

    public static ProductBuilder Product => new();
    public static PaymentBuilder Payment => new();

    public static ProductDto CreateProductDto(int id = 1, string name = "Laptop")
        => new(id, name, "Description", 1000m, 10, DateTime.UtcNow);

    public static RoleBuilder Role => new();
}
