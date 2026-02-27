using OrderPaymentSystem.Domain.Abstract;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Entities;
using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Shared.Exceptions;

namespace OrderPaymentSystem.Domain.Entities;

/// <summary>
/// Товар
/// </summary>
public class Product : BaseEntity<int>, IAuditable, IStockInfo
{
    /// <summary>
    /// Название товара
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Описание товара
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// Стоимость единицы товара
    /// </summary>
    public decimal Price { get; private set; }

    /// <inheritdoc/>
    public DateTime CreatedAt { get; private set; }

    /// <inheritdoc/>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Внутренняя коллекция элементов заказа
    /// </summary>
    private readonly List<OrderItem> _orderItems = [];

    /// <summary>
    /// Элементы заказа, только для чтения.
    /// </summary>
    public virtual IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    /// <summary>
    /// Элементы корзины в которых есть товар
    /// </summary>
    public ICollection<BasketItem> BasketItems { get; private set; } = [];

    /// <summary>
    /// Количество товара на складе
    /// </summary>
    public int StockQuantity { get; private set; }

    /// <summary>
    /// Поле которое хранит версию кортежа. Для контроля паралеллизма.
    /// </summary>
    public uint RowVersion { get; private set; }

    private Product() { }

    private Product(int id, string name, string description, decimal price, int stockQuantity)
    {
        Id = id;
        Name = name;
        Description = description;
        Price = price;
        StockQuantity = stockQuantity;
    }

    private Product(string name, string description, decimal price, int stockQuantity)
    {
        Name = name;
        Description = description;
        Price = price;
        StockQuantity = stockQuantity;
    }

    /// <summary>
    /// Создать существующий товар. Используется только для тестов
    /// </summary>
    internal static Product CreateExisting(int id, string name, string description, decimal price, int stockQuantity)
    {
        Validate(name, price);

        return new Product(id, name, description, price, stockQuantity);
    }

    /// <summary>
    /// Создать товар
    /// </summary>
    /// <param name="name">Название товара</param>
    /// <param name="description">Описание товара</param>
    /// <param name="price">Стоимость единицы товара</param>
    /// <param name="stockQuantity">Количество товара на складе</param>
    /// <returns>Созданный товар</returns>
    public static Product Create(string name, string description, decimal price, int stockQuantity)
    {
        Validate(name, price);

        return new Product(name, description, price, stockQuantity);
    }

    /// <summary>
    /// Обновить данные товара
    /// </summary>
    /// <param name="name">Название товара</param>
    /// <param name="description">Описание товара</param>
    /// <param name="newPrice">Новая стоимость единицы товара</param>
    /// <param name="stockQuantity">Количество товара на складе</param>
    public void UpdateDetails(string name, string description, decimal newPrice, int stockQuantity)
    {
        Validate(name, newPrice);

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
            throw new BusinessException(DomainErrors.Product.PricePositive());

        Price = newPrice;
    }

    /// <inheritdoc/>
    public bool IsStockQuantityAvailable(int requestedQuantity)
    {
        return StockQuantity >= requestedQuantity;
    }

    /// <summary>
    /// Снизить количество товара на складе.
    /// </summary>
    /// <param name="quantityToReduce">Количество для понижения</param>
    /// <exception cref="BusinessException"></exception>
    public void ReduceStockQuantity(int quantityToReduce)
    {
        if (quantityToReduce <= 0)
        {
            throw new BusinessException(DomainErrors.Product.StockPositive());
        }

        if (StockQuantity < quantityToReduce)
        {
            throw new BusinessException(DomainErrors.Product.StockNotAvailable(quantityToReduce, Id));
        }

        StockQuantity -= quantityToReduce;
    }

    /// <summary>
    /// Валидировать входные данные
    /// </summary>
    /// <param name="name">Название товара</param>
    /// <param name="price">Цена товара</param>
    /// <exception cref="BusinessException"></exception>
    private static void Validate(string name, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new BusinessException(DomainErrors.Validation.Required(nameof(name)));
        if (price <= 0) throw new BusinessException(DomainErrors.Product.PricePositive());
    }
}