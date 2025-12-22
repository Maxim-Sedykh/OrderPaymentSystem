using OrderPaymentSystem.Domain.Interfaces.Entities;
using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Domain.Entities;

/// <summary>
/// Позиция заказа.
/// </summary>
public class OrderItem : IEntityId<long>
{
    /// <summary>
    /// Id позиции заказа
    /// </summary>
    public long Id { get; protected set; }

    /// <summary>
    /// Id заказа
    /// </summary>
    public long OrderId { get; protected set; }

    /// <summary>
    /// Id товара
    /// </summary>
    public int ProductId { get; protected set; }

    /// <summary>
    /// Количество товара
    /// </summary>
    public int Quantity { get; protected set; }

    /// <summary>
    /// Общая стоимость позиции заказа
    /// </summary>
    public decimal ItemTotalSum { get; protected set; }

    /// <summary>
    /// Стоимость товара
    /// </summary>
    public decimal ProductPrice { get; protected set; }

    /// <summary>
    /// Товар
    /// </summary>
    public Product Product { get; protected set; }

    /// <summary>
    /// Заказ
    /// </summary>
    public Order Order { get; protected set; }

    protected OrderItem() { }

    /// <summary>
    /// Создать позицию заказа
    /// </summary>
    /// <param name="productId">Id товара</param>
    /// <param name="quantity">Количество товара</param>
    /// <param name="productPrice">Стоимость товара</param>
    /// <returns>Результат операции создания</returns>
    public static DataResult<OrderItem> Create(int productId, int quantity, int productPrice)
    {
        if (quantity <= 0) return DataResult<OrderItem>.Failure(7001, "Quantity must be positive.");

        return DataResult<OrderItem>.Success(new OrderItem
        {
            Id = default,
            ProductId = productId,
            Quantity = quantity,
            ProductPrice = productPrice,
            ItemTotalSum = productPrice * quantity
        });
    }
}
