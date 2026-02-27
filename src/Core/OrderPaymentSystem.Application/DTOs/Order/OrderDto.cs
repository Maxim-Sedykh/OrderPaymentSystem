using OrderPaymentSystem.Application.DTOs.OrderItem;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.ValueObjects;

namespace OrderPaymentSystem.Application.DTOs.Order;

/// <summary>
/// Модель данных для представления заказа
/// </summary>
public class OrderDto
{
    /// <summary>
    /// Id заказа
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Id пользователя
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Id платежа
    /// </summary>
    public long? PaymentId { get; set; }

    /// <summary>
    /// Общая стоимость заказа
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Статус заказа
    /// </summary>
    public OrderStatus Status { get; set; }

    /// <summary>
    /// Адрес доставки заказа
    /// </summary>
    public Address? DeliveryAddress { get; set; }

    /// <summary>
    /// Элементы заказа
    /// </summary>
    public List<OrderItemDto> Items { get; set; } = [];
}
