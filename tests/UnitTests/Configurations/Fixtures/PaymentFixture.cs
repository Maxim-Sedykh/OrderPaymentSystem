using MapsterMapper;
using Moq;
using OrderPaymentSystem.Application.DTOs.Payment;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Application.Services;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Shared.Specifications;
using OrderPaymentSystem.UnitTests.Configurations.Factories;

namespace OrderPaymentSystem.UnitTests.Configurations.Fixtures;

internal class PaymentFixture
{
    public Mock<IUnitOfWork> Uow { get; } = new();
    public Mock<IPaymentRepository> PaymentRepo { get; } = new();
    public Mock<IOrderRepository> OrderRepo { get; } = new();
    public Mock<IMapper> Mapper { get; } = new();

    public PaymentService Service { get; }

    public PaymentFixture()
    {
        Uow.Setup(u => u.Payments).Returns(PaymentRepo.Object);
        Uow.Setup(u => u.Orders).Returns(OrderRepo.Object);
        Mapper.Setup(u => u.Map<PaymentDto>(It.IsAny<Payment>())).Returns(TestDataFactory.CreatePaymentDto());

        Service = new PaymentService(Uow.Object, Mapper.Object);
    }

    public PaymentFixture SetupOrder(Order order)
    {
        OrderRepo.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Order>>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(order);
        return this;
    }

    public PaymentFixture SetupPayment(Payment payment)
    {
        PaymentRepo.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Payment>>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(payment);
        return this;
    }

    public PaymentFixture SetupPaymentExistence(bool exists)
    {
        PaymentRepo.Setup(r => r.AnyAsync(It.IsAny<BaseSpecification<Payment>>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(exists);
        return this;
    }

    public PaymentFixture SetupProjectedPayment(PaymentDto dto)
    {
        PaymentRepo.Setup(r => r.GetProjectedAsync<PaymentDto>(It.IsAny<BaseSpecification<Payment>>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(dto);
        return this;
    }

    public void VerifySaved() => Uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    public void VerifyNotSaved() => Uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
}
