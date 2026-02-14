using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Enum;

namespace OrderPaymentSystem.UnitTests.Configurations.Factories.Builders;


public class PaymentBuilder
{
    private long _id = 1;
    private long _orderId = 1;
    private decimal _amountToPay = 1000m;
    private decimal _amountPaid = 0m;
    private PaymentStatus _status = PaymentStatus.Pending;

    public PaymentBuilder WithId(long id) { _id = id; return this; }
    public PaymentBuilder WithOrder(long orderId) { _orderId = orderId; return this; }
    public PaymentBuilder ToPay(decimal amount) { _amountToPay = amount; return this; }
    public PaymentBuilder Paid(decimal amount) { _amountPaid = amount; return this; }
    public PaymentBuilder WithStatus(PaymentStatus status) { _status = status; return this; }

    public Payment Build() => Payment.CreateExisting(_id, _orderId, _amountToPay, _amountPaid, PaymentMethod.Cash, _status);
}
