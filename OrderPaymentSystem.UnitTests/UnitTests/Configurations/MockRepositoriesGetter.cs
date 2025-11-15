using MockQueryable.Moq;
using Moq;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Interfaces.Repositories;

namespace OrderPaymentSystem.Tests.UnitTests.Configurations;

public static class MockRepositoriesGetter
{
    public static Mock<IBaseRepository<Product>> GetMockProductRepository()
    {
        var mock = new Mock<IBaseRepository<Product>>();

        var products = GetProducts().CreateMockDbSet();
        mock.Setup(x => x.GetAll()).Returns(() => products.Object);

        return mock;
    }

    public static IQueryable<Product> GetProducts()
    {
        return new List<Product>()
        {
            new() { 
                Id = 1, 
                ProductName = "Test Product #1", 
                Description = "Test description of product #1", 
                Cost = 5000, 
                CreatedAt = DateTime.UtcNow 
            },
            new() {
                Id = 2,
                ProductName = "Test Product #2",
                Description = "Test description of product #2",
                Cost = 1500,
                CreatedAt = DateTime.UtcNow
            },
        }.AsQueryable();
    }
}
