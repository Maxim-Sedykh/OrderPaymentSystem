namespace OrderPaymentSystem.Domain.Enum;

public enum PaymentStatus
{
    /// <summary>
    /// В ожидании
    /// </summary>
    Pending,

    /// <summary>
    /// Успешно
    /// </summary>
    Succeeded,

    /// <summary>
    /// Неуспешно
    /// </summary>
    Failed,

    /// <summary>
    /// Возвращен
    /// </summary>
    Refunded,

    /// <summary>
    /// Частично возвращен
    /// </summary>
    PartiallyRefunded
}
