using OrderPaymentSystem.Application.DTOs.OrderItem;
using OrderPaymentSystem.Domain.ValueObjects;

namespace OrderPaymentSystem.Application.DTOs.Order;

/// <summary>
/// Модель данных для создания заказа
/// </summary>
public record CreateOrderDto
{
    /// <summary>
    /// Элементы заказа в виде модели данных
    /// </summary>
    public List<CreateOrderItemDto> OrderItems { get; set; } = [];

    /// <summary>
    /// Адрес доставки
    /// </summary>
    public Address? DeliveryAddress { get; set; }
}
