namespace OrderPaymentSystem.Application.DTOs.OrderItem;

/// <summary>
/// Модель данных для представления элемента заказа
/// </summary>
public class OrderItemDto
{
    /// <summary>
    /// Id элемента заказа
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Id заказа, которому принадлежит элемент
    /// </summary>
    public long OrderId { get; set; }

    /// <summary>
    /// Id товара
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Количество товара
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Общая стоимость элемента заказа
    /// </summary>
    public decimal ItemTotalSum { get; set; }

    /// <summary>
    /// Цена за единицу товара
    /// </summary>
    public decimal ProductPrice { get; set; }
}
