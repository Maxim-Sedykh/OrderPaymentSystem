using OrderPaymentSystem.Domain.Exceptions;
using OrderPaymentSystem.Domain.Interfaces.Entities;

namespace OrderPaymentSystem.Domain.Entities;

/// <summary>
/// Товар
/// </summary>
public class Product : IEntityId<int>, IAuditable, IStockInfo
{
    /// <summary>
    /// Id товара
    /// </summary>
    public int Id { get; protected set; }

    /// <summary>
    /// Название товара
    /// </summary>
    public string Name { get; protected set; }

    /// <summary>
    /// Описание товара
    /// </summary>
    public string Description { get; protected set; }

    /// <summary>
    /// Стоимость единицы товара
    /// </summary>
    public decimal Price { get; protected set; }

    /// <inheritdoc/>
    public DateTime CreatedAt { get; protected set; }

    /// <inheritdoc/>
    public DateTime? UpdatedAt { get; protected set; }

    private readonly List<OrderItem> _orderItems = new();
    public virtual IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    /// <summary>
    /// Элементы корзины в которых есть товар
    /// </summary>
    public ICollection<BasketItem> BasketItems { get; protected set; } = [];

    public int StockQuantity { get; private set; }

    public byte[] RowVersion { get; private set; }

    protected Product() { }

    /// <summary>
    /// Создать товар
    /// </summary>
    /// <param name="name">Название товара</param>
    /// <param name="description">Описание товара</param>
    /// <param name="price">Стоимость единицы товара</param>
    /// <returns>Созданный товар</returns>
    public static Product Create(string name, string description, decimal price, int stockQuantity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessException(2001, "Product name cannot be empty.");

        if (price <= 0)
            throw new BusinessException(2002, "Product price must be positive.");

        return new Product
        {
            Id = default,
            Name = name,
            Description = description,
            Price = price,
            StockQuantity = stockQuantity
        };
    }

    /// <summary>
    /// Обновить данные товара
    /// </summary>
    /// <param name="name">Название товара</param>
    /// <param name="description">Описание товара</param>
    /// <param name="newPrice">Новая стоимость единицы товара</param>
    public void UpdateDetails(string name, string description, decimal newPrice, int stockQuantity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessException(2003, "Product name cannot be empty.");

        if (newPrice <= 0)
            throw new BusinessException(2004, "Product price must be positive.");

        Name = name;
        Description = description;
        Price = newPrice;
        StockQuantity = stockQuantity;
    }

    /// <summary>
    /// Изменить стоимость единицы товара
    /// </summary>
    /// <param name="newPrice">Новая стоимость единицы товара</param>
    public void ChangePrice(decimal newPrice)
    {
        if (newPrice <= 0)
            throw new BusinessException(2005, "Product price must be positive.");

        Price = newPrice;
    }

    public bool IsStockQuantityAvailable(int requestedQuantity)
    {
        return StockQuantity >= requestedQuantity;
    }

    public void ReduceStockQuantity(int quantityToReduce)
    {
        if (quantityToReduce <= 0)
        {
            throw new BusinessException(20055, "Stock quantity must be positive.");
        }

        if (StockQuantity < quantityToReduce)
        {
            throw new BusinessException(20055, "Not enough stock quantity.");
        }

        StockQuantity -= quantityToReduce;
    }
}