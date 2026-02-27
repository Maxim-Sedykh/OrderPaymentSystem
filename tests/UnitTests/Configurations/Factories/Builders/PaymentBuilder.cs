using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Enum;

namespace OrderPaymentSystem.UnitTests.Configurations.Factories.Builders;

/// <summary>
/// Билдер для построения мокового платежа
/// </summary>
public class PaymentBuilder
{
    private long _id = 1;
    private long _orderId = 1;
    private decimal _amountToPay = 1000m;
    private decimal _amountPaid = 0m;
    private PaymentStatus _status = PaymentStatus.Pending;

    /// <summary>
    /// Добавить цену для оплаты
    /// </summary>
    public PaymentBuilder ToPay(decimal amount) { _amountToPay = amount; return this; }

    /// <summary>
    /// Добавить статус платежа
    /// </summary>
    public PaymentBuilder WithStatus(PaymentStatus status) { _status = status; return this; }

    /// <summary>
    /// Построить, создать объект.
    /// </summary>
    /// <returns>Созданный платёж</returns>
    public Payment Build() => Payment.CreateExisting(_id, _orderId, _amountPaid, _amountToPay, PaymentMethod.Cash, _status);
}
