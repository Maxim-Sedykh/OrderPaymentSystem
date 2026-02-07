using FluentAssertions;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Shared.Exceptions;
using Xunit;

namespace OrderPaymentSystem.UnitTests.EntityTests;

public class PaymentTests
{
    [Fact]
    public void ProcessPayment_WithCorrectAmount_ShouldSucceed()
    {
        // Arrange
        var payment = Payment.Create(1, 1000m, 1000m, PaymentMethod.Cash);

        // Act
        payment.ProcessPayment(1000m, 0m);

        // Assert
        payment.Status.Should().Be(PaymentStatus.Succeeded);
        payment.CashChange.Should().Be(0m);
    }

    [Fact]
    public void ProcessPayment_WithInsufficientAmount_ShouldThrowBusinessException()
    {
        // Arrange
        var payment = Payment.Create(1, 1000m, 1000m, PaymentMethod.Cash);

        // Act
        Action act = () => payment.ProcessPayment(500m, 0m);

        // Assert
        act.Should().Throw<BusinessException>();
    }

    [Fact]
    public void ProcessPayment_WrongCashChange_ShouldThrowBusinessException()
    {
        // Arrange
        var payment = Payment.Create(1, 1000m, 800m, PaymentMethod.Cash);

        // Act
        // Оплачено 1000, цена 800, сдача должна быть 200, а мы передаем 100
        Action act = () => payment.ProcessPayment(1000m, 100m);

        // Assert
        act.Should().Throw<BusinessException>();
    }
}
