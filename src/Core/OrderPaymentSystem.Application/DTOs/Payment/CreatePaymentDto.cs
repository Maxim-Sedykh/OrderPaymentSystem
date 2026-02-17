using OrderPaymentSystem.Domain.Enum;

namespace OrderPaymentSystem.Application.DTOs.Payment;

public class CreatePaymentDto
{
    public long OrderId { get; set; }
    public decimal AmountPaid { get; set; }
    public PaymentMethod Method { get; set; }
}
