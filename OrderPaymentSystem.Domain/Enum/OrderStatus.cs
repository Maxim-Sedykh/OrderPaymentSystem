namespace OrderPaymentSystem.Domain.Enum;

/// <summary>
/// Статус заказа
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// В ожидании
    /// </summary>
    Pending,

    /// <summary>
    /// Вернуть средства за заказ
    /// </summary>
    Refunded,

    /// <summary>
    /// Размещен
    /// </summary>
    Placed,

    /// <summary>
    /// В обработке
    /// </summary>
    Processing,

    /// <summary>
    /// Отправлен
    /// </summary>
    Shipped,

    /// <summary>
    /// Доставлен
    /// </summary>
    Delivered,

    /// <summary>
    /// Отменен
    /// </summary>
    Cancelled
}
