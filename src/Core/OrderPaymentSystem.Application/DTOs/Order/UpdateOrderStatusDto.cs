using OrderPaymentSystem.Domain.Enum;

namespace OrderPaymentSystem.Application.DTOs.Order;

/// <summary>
/// Модель данных для обновления статуса заказа
/// </summary>
public record UpdateOrderStatusDto
{
    /// <summary>
    /// Новый статус для заказа
    /// </summary>
    public OrderStatus NewStatus { get; set; }
}
