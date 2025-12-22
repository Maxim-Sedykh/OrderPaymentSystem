using OrderPaymentSystem.Domain.Interfaces.Entities;
using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Domain.Entities;

/// <summary>
/// Товар
/// </summary>
public class Product : IEntityId<int>, IAuditable
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

    /// <summary>
    /// Элементы заказов в которых есть товар
    /// </summary>
    public ICollection<OrderItem> OrderItems { get; protected set; }

    /// <summary>
    /// Элементы корзины в которых есть товар
    /// </summary>
    public ICollection<BasketItem> BasketItems { get; protected set; }

    protected Product() { }

    /// <summary>
    /// Создать товар
    /// </summary>
    /// <param name="name">Название товара</param>
    /// <param name="description">Описание товара</param>
    /// <param name="price">Стоимость единицы товара</param>
    /// <returns>Результат создания товара</returns>
    public static DataResult<Product> Create(string name, string description, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name)) return DataResult<Product>.Failure(2001, "Product name cannot be empty.");
        if (price <= 0) return DataResult<Product>.Failure(2002, "Product price must be positive.");

        return DataResult<Product>.Success(new Product
        {
            Id = default,
            Name = name,
            Description = description,
            Price = price
        });
    }

    /// <summary>
    /// Обновить данные товара
    /// </summary>
    /// <param name="name">Название товара</param>
    /// <param name="description">Описание товара</param>
    /// <param name="newPrice">Новая стоимость единицы товара</param>
    /// <returns>Результат обновления</returns>
    public BaseResult UpdateDetails(string name, string description, decimal newPrice)
    {
        if (string.IsNullOrWhiteSpace(name)) return BaseResult.Failure(2003, "Product name cannot be empty.");
        if (newPrice <= 0) return BaseResult.Failure(2004, "Product price must be positive.");

        Name = name;
        Description = description;
        Price = newPrice;

        return BaseResult.Success();
    }

    /// <summary>
    /// Изменить стоимость единицы товара
    /// </summary>
    /// <param name="newPrice">Новая стоимость единицы товара</param>
    /// <returns>Результат обновления</returns>
    public BaseResult ChangePrice(decimal newPrice)
    {
        if (newPrice <= 0) return BaseResult.Failure(2005, "Product price must be positive.");
        Price = newPrice;

        return BaseResult.Success();
    }
}