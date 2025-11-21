using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces.Entities;
using OrderPaymentSystem.Domain.ValueObjects;

namespace OrderPaymentSystem.Domain.Entities;

public class Payment : IEntityId<long>, IAuditable
{
    public long Id { get; set; }

    public long BasketId { get; set; }

    public Basket Basket { get; set; }

    public decimal CostOfOrders { get; set; }

    public decimal AmountOfPayment { get; set; }

    public Address DeliveryAddress { get; set; }

    public decimal CashChange { get; set; }

    public PaymentMethod PaymentMethod { get; set; }

    public DateTime CreatedAt { get; set; }

    public long CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    public ICollection<Order> Orders { get; set; }
}
