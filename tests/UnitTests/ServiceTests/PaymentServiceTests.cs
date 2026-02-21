using FluentAssertions;
using Moq;
using OrderPaymentSystem.Application.DTOs.Payment;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Shared.Exceptions;
using OrderPaymentSystem.UnitTests.Configurations.Factories;
using OrderPaymentSystem.UnitTests.Configurations.Fixtures;
using Xunit;

namespace OrderPaymentSystem.UnitTests.ServiceTests;

public class PaymentServiceTests
{
    private readonly PaymentFixture _fixture;

    public PaymentServiceTests()
    {
        _fixture = new PaymentFixture();
    }

    [Fact]
    public async Task CreateAsync_WhenValid_ShouldCreatePayment()
    {
        // Arrange
        var order = TestDataFactory.Order.WithItems(TestDataFactory.OrderItem.Build()).Build();
        var dto = new CreatePaymentDto { OrderId = order.Id, AmountPaid = 1000m, Method = PaymentMethod.Cash };

        _fixture.SetupOrder(order)
                .SetupPaymentExistence(false);

        // Act
        var result = await _fixture.Service.CreateAsync(dto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _fixture.PaymentRepo.Verify(r => r.CreateAsync(
            It.IsAny<Payment>(),
            It.IsAny<CancellationToken>()), Times.Once);
        _fixture.VerifySaved();
    }

    [Fact]
    public async Task CreateAsync_WhenPaymentAlreadyExists_ShouldReturnError()
    {
        // Arrange
        _fixture.SetupPaymentExistence(true);

        // Act
        var result = await _fixture.Service.CreateAsync(new CreatePaymentDto { OrderId = 1 });

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(DomainErrors.Payment.AlreadyExists(1));
        _fixture.VerifyNotSaved();
    }

    [Fact]
    public async Task CompletePaymentAsync_WhenValidAmount_ShouldSucceed()
    {
        // Arrange
        var payment = TestDataFactory.Payment.ToPay(1000m).WithStatus(PaymentStatus.Pending).Build();
        var completeDto = new CompletePaymentDto { AmountPaid = 1200m, CashChange = 200m };

        _fixture.SetupPayment(payment);

        // Act
        var result = await _fixture.Service.CompletePaymentAsync(payment.Id, completeDto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        payment.Status.Should().Be(PaymentStatus.Succeeded);
        payment.AmountPaid.Should().Be(1200m);
        payment.CashChange.Should().Be(200m);
        _fixture.VerifySaved();
    }

    [Fact]
    public async Task CompletePaymentAsync_WhenAmountIsInsufficient_ShouldThrowBusinessException()
    {
        // Arrange
        var payment = TestDataFactory.Payment.ToPay(1000m).Build();
        var completeDto = new CompletePaymentDto { AmountPaid = 500m };

        _fixture.SetupPayment(payment);

        // Act
        Func<Task> act = () => _fixture.Service.CompletePaymentAsync(payment.Id, completeDto);

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
                  .WithMessage(DomainErrors.Payment.NotEnoughAmount(500m, 1000m).Message);
        _fixture.VerifyNotSaved();
    }

    [Fact]
    public async Task GetByIdAsync_WhenExists_ShouldReturnDto()
    {
        // Arrange
        var expectedDto = new PaymentDto { Id = 1 };
        _fixture.SetupProjectedPayment(expectedDto);

        // Act
        var result = await _fixture.Service.GetByIdAsync(1);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(expectedDto);
    }
}
