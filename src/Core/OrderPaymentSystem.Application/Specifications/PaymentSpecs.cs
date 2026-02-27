using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Shared.Specifications;

namespace OrderPaymentSystem.Application.Specifications;

/// <summary>
/// Спецификации для сущности "Платёж"
/// </summary>
internal static class PaymentSpecs
{
    public static BaseSpecification<Payment> ById(long id)
        => new(x => x.Id == id);

    public static BaseSpecification<Payment> ByOrderId(long orderId)
        => new(x => x.OrderId == orderId);
}
