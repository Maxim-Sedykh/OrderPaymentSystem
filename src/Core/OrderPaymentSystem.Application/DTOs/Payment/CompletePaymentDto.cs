namespace OrderPaymentSystem.Application.DTOs.Payment;

/// <summary>
/// Модель данных для завершения платежа
/// </summary>
public class CompletePaymentDto
{
    /// <summary>
    /// Сдача
    /// </summary>
    public decimal CashChange { get; set; }

    /// <summary>
    /// Сколько заплатил пользователь
    /// </summary>
    public decimal AmountPaid { get; set; }
}
