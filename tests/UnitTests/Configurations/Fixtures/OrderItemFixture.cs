using MapsterMapper;
using Microsoft.Extensions.Logging;
using Moq;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Application.Services;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Shared.Specifications;

namespace OrderPaymentSystem.UnitTests.Configurations.Fixtures;

public class OrderItemFixture
{
    public Mock<IUnitOfWork> Uow { get; } = new();
    public Mock<IMapper> Mapper { get; } = new();
    public Mock<IOrderRepository> OrderRepo { get; } = new();
    public Mock<IProductRepository> ProductRepo { get; } = new();
    public Mock<IOrderItemRepository> OrderItemRepo { get; } = new();

    public OrderItemService Service { get; }

    public OrderItemFixture()
    {
        Uow.Setup(u => u.Orders).Returns(OrderRepo.Object);
        Uow.Setup(u => u.Products).Returns(ProductRepo.Object);
        Uow.Setup(u => u.OrderItems).Returns(OrderItemRepo.Object);

        Service = new OrderItemService(Uow.Object, Mapper.Object, Mock.Of<ILogger<OrderItemService>>());
    }

    public OrderItemFixture SetupOrder(Order order)
    {
        OrderRepo.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Order>>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(order);
        return this;
    }

    public OrderItemFixture SetupProduct(Product product)
    {
        ProductRepo.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Product>>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(product);
        return this;
    }

    public OrderItemFixture SetupOrderItem(OrderItem item)
    {
        OrderItemRepo.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<OrderItem>>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(item);
        return this;
    }

    public OrderItemFixture SetupMapping<TSource, TDest>(TDest dest)
    {
        Mapper.Setup(m => m.Map<TDest>(It.IsAny<TSource>())).Returns(dest);
        return this;
    }

    public void VerifySaved() => Uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    public void VerifyNotSaved() => Uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
}
