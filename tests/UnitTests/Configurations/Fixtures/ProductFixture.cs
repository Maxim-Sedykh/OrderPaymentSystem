using MapsterMapper;
using Microsoft.Extensions.Logging;
using Moq;
using OrderPaymentSystem.Application.Interfaces.Cache;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Application.Services;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Shared.Specifications;

namespace OrderPaymentSystem.UnitTests.Configurations.Fixtures;

internal class ProductFixture
{
    public Mock<IUnitOfWork> Uow { get; } = new();
    public Mock<IProductRepository> ProductRepo { get; } = new();
    public Mock<ICacheService> Cache { get; } = new();
    public Mock<IMapper> Mapper { get; } = new();

    public ProductService Service { get; }

    public ProductFixture()
    {
        Uow.Setup(u => u.Products).Returns(ProductRepo.Object);

        Service = new ProductService(
            Uow.Object,
            Mock.Of<ILogger<ProductService>>(),
            Cache.Object,
            Mapper.Object);
    }

    public ProductFixture SetupProduct(Product product)
    {
        ProductRepo.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Product>>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(product);
        return this;
    }

    public ProductFixture SetupProductExistence(bool exists)
    {
        ProductRepo.Setup(r => r.AnyAsync(It.IsAny<BaseSpecification<Product>>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(exists);
        return this;
    }

    public ProductFixture SetupCacheGet<T>(string key, T value) where T : class
    {
        Cache.Setup(c => c.GetOrCreateAsync(
                It.Is<string>(k => k == key),
                It.IsAny<Func<CancellationToken, Task<T>>>()))
             .ReturnsAsync(value);
        return this;
    }

    public ProductFixture SetupMapping<TSrc, TDest>(TDest dest)
    {
        Mapper.Setup(m => m.Map<TDest>(It.IsAny<TSrc>())).Returns(dest);
        return this;
    }

    public void VerifySaved() => Uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    public void VerifyCacheRemoved(string key) => Cache.Verify(c => c.RemoveAsync(key, It.IsAny<CancellationToken>()), Times.Once);
}
