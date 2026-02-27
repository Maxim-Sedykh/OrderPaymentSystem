using FluentAssertions;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Shared.Exceptions;

namespace OrderPaymentSystem.UnitTests.EntityTests;

/// <summary>
/// Тесты сущности <see cref="Payment"/>
/// </summary>
public class PaymentTests
{
    /// <summary>
    /// Обработка платежа с корректным денежным количеством должно быть успешно
    /// </summary>
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

    /// <summary>
    /// Обработка платежа с недостающим денежным количеством должно выбрасывать BusinessException
    /// </summary>
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

    /// <summary>
    /// Обработка платежа с неправильной сдачей должно выбрасывать BusinessException
    /// </summary>
    [Fact]
    public void ProcessPayment_WrongCashChange_ShouldThrowBusinessException()
    {
        // Arrange
        var payment = Payment.Create(1, 1000m, 800m, PaymentMethod.Cash);

        // Act
        Action act = () => payment.ProcessPayment(1000m, 100m);

        // Assert
        act.Should().Throw<BusinessException>();
    }
}
