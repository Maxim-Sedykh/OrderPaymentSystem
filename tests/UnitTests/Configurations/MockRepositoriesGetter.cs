using MockQueryable.Moq;
using Moq;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Interfaces.Repositories.Base;

namespace OrderPaymentSystem.UnitTests.Configurations;

public static class MockRepositoriesGetter
{
    public static Mock<IBaseRepository<Product>> GetMockProductRepository()
    {
        var mock = new Mock<IBaseRepository<Product>>();

        var products = GetProducts().CreateMockDbSet();
        mock.Setup(x => x.GetQueryable()).Returns(() => products.Object);

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
