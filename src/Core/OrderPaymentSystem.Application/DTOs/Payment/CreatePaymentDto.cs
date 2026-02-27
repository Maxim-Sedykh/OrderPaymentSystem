using OrderPaymentSystem.Domain.Enum;

namespace OrderPaymentSystem.Application.DTOs.Payment;

/// <summary>
/// Модель для создания платежа
/// </summary>
public class CreatePaymentDto
{
    /// <summary>
    /// Id заказа
    /// </summary>
    public long OrderId { get; set; }

    /// <summary>
    /// Количество которое заплатил пользователь
    /// </summary>
    public decimal AmountPaid { get; set; }

    /// <summary>
    /// Способ платежа
    /// </summary>
    public PaymentMethod Method { get; set; }
}
