using MapsterMapper;
using Moq;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Application.Services;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Shared.Specifications;

namespace OrderPaymentSystem.UnitTests.Configurations.Fixtures;

public class BasketItemFixture
{
    public Mock<IUnitOfWork> Uow { get; } = new();
    public Mock<IMapper> Mapper { get; } = new();
    public Mock<IProductRepository> ProductRepo { get; } = new();
    public Mock<IBasketItemRepository> BasketRepo { get; } = new();

    public BasketItemService Service { get; }

    public BasketItemFixture()
    {
        Uow.Setup(u => u.Products).Returns(ProductRepo.Object);
        Uow.Setup(u => u.BasketItems).Returns(BasketRepo.Object);

        Service = new BasketItemService(Uow.Object, Mapper.Object);
    }

    public BasketItemFixture SetupProduct(Product product)
    {
        ProductRepo.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Product>>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(product);
        return this;
    }

    public BasketItemFixture SetupBasketItem(BasketItem item)
    {
        BasketRepo.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<BasketItem>>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(item);
        return this;
    }

    public BasketItemFixture SetupMapping<TSource, TDest>(TDest destination)
    {
        Mapper.Setup(m => m.Map<TDest>(It.IsAny<TSource>())).Returns(destination);
        return this;
    }

    public void VerifyBasketItemCreated()
        => BasketRepo.Verify(r => r.CreateAsync(It.IsAny<BasketItem>(), It.IsAny<CancellationToken>()), Times.Once);

    public void VerifyBasketItemRemoved(BasketItem item)
        => BasketRepo.Verify(r => r.Remove(item), Times.Once);

    public void VerifySaved()
        => Uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

    public void VerifyNotSaved()
        => Uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
}
