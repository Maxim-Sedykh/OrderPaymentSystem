namespace OrderPaymentSystem.Application.DTOs.OrderItem;

/// <summary>
/// Модель для обновления элемента заказа
/// </summary>
public record UpdateOrderItemDto
{
    /// <summary>
    /// Id товара
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Новое количество товара
    /// </summary>
    public int NewQuantity { get; set; }
}
