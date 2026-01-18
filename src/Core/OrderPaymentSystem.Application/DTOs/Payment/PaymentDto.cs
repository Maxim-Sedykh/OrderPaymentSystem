using OrderPaymentSystem.Domain.Enum;

namespace OrderPaymentSystem.Application.DTOs.Payment;

public class PaymentDto
{
    public long Id { get; set; }
    public long OrderId { get; set; }
    public decimal AmountToPay { get; set; }
    public decimal? AmountPayed { get; set; }
    public decimal? CashChange { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; }
}
