using MapsterMapper;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Application.Services.Orders;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Shared.Specifications;

namespace OrderPaymentSystem.UnitTests.Configurations.Fixtures;

internal class OrderFixture
{
    public Mock<IUnitOfWork> Uow { get; } = new();
    public Mock<IMapper> Mapper { get; } = new();
    public Mock<IOrderRepository> OrderRepo { get; } = new();
    public Mock<IPaymentRepository> PaymentRepo { get; } = new();
    public Mock<IProductRepository> ProductRepo { get; } = new();
    public Mock<IDbContextTransaction> Transaction { get; } = new();

    public OrderService Service { get; }

    public OrderFixture()
    {
        Uow.Setup(u => u.Orders).Returns(OrderRepo.Object);
        Uow.Setup(u => u.Payments).Returns(PaymentRepo.Object);
        Uow.Setup(u => u.Products).Returns(ProductRepo.Object);
        Uow.Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Transaction.Object);

        Service = new OrderService(Mock.Of<ILogger<OrderService>>(), Mapper.Object, Uow.Object);
    }

    public OrderFixture SetupOrder(Order order)
    {
        OrderRepo.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Order>>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(order);
        return this;
    }

    public OrderFixture SetupPayment(Payment payment)
    {
        PaymentRepo.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Payment>>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(payment);
        return this;
    }

    public OrderFixture SetupProductsDictionary(IEnumerable<Product> products)
    {
        ProductRepo.Setup(r => r.GetProductsAsDictionaryByIdAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(products.ToDictionary(p => p.Id));
        return this;
    }

    public OrderFixture SetupMapping<TSrc, TDest>(TDest dest)
    {
        Mapper.Setup(m => m.Map<TDest>(It.IsAny<TSrc>())).Returns(dest);
        return this;
    }

    public void VerifySaved() => Uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    public void VerifyNotSaved() => Uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    public void VerifyTransactionCommitted() => Transaction.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
}
