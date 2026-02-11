using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.ValueObjects;

namespace OrderPaymentSystem.UnitTests.Configurations
{
    public static class TestDataFactory
    {
        public static Address CreateAddress() => new("Street", "City", "101", "Country");

        public static Product CreateProduct(int id = 1, decimal price = 100m, int stock = 10) =>
            Product.CreateExisting(id, "Test Product", "Description", price, stock);

        public static User CreateUser(Guid? id = null) =>
            User.CreateExisting(id ?? Guid.NewGuid(), "testuser", "hashed_password");

        public static Order CreateOrder(long id = 1, OrderStatus status = OrderStatus.Pending) =>
            Order.CreateExisting(
                id,
                Guid.NewGuid(),
                CreateAddress(),
                new List<OrderItem>(),
                0m,
                status);
    }
}
