using OrderPaymentSystem.Application.DTOs.OrderItem;

namespace OrderPaymentSystem.Application.DTOs.Order;

/// <summary>
/// Модель данных для массового обновления элементов заказа
/// </summary>
public class UpdateBulkOrderItemsDto
{
    /// <summary>
    /// Элементы заказа
    /// </summary>
    public List<UpdateOrderItemDto> Items { get; set; } = [];
}
