using OrderPaymentSystem.Domain.Enum;

namespace OrderPaymentSystem.Application.DTOs.Payment;

/// <summary>
/// Модель для представления платежа
/// </summary>
public class PaymentDto
{
    /// <summary>
    /// Id платежа
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Id заказа прикреплённого к платежу
    /// </summary>
    public long OrderId { get; set; }

    /// <summary>
    /// Денежная сумма которую нужно заплатить
    /// </summary>
    public decimal AmountToPay { get; set; }

    /// <summary>
    /// Денежная сумма которая была заплаченна
    /// </summary>
    public decimal? AmountPayed { get; set; }

    /// <summary>
    /// Сдача
    /// </summary>
    public decimal? CashChange { get; set; }

    /// <summary>
    /// Способ платежа
    /// </summary>
    public PaymentMethod Method { get; set; }

    /// <summary>
    /// Статус платежа
    /// </summary>
    public PaymentStatus Status { get; set; }
}
