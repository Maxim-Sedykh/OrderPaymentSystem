namespace OrderPaymentSystem.Application.DTOs.OrderItem;

/// <summary>
/// Модель для создания элемента заказа
/// </summary>
public class CreateOrderItemDto
{
    /// <summary>
    /// Id товара
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Количество товара
    /// </summary>
    public int Quantity { get; set; }
}
