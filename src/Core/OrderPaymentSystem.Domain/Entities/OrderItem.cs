using OrderPaymentSystem.Domain.Abstract;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Entities;
using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Shared.Exceptions;

namespace OrderPaymentSystem.Domain.Entities;

/// <summary>
/// Позиция заказа.
/// </summary>
public class OrderItem : BaseEntity<long>
{
    /// <summary>
    /// Id заказа
    /// </summary>
    public long OrderId { get; private set; }

    /// <summary>
    /// Id товара
    /// </summary>
    public int ProductId { get; private set; }

    /// <summary>
    /// Количество товара
    /// </summary>
    public int Quantity { get; private set; }

    /// <summary>
    /// Общая стоимость позиции заказа
    /// </summary>
    public decimal ItemTotalSum { get; private set; }

    /// <summary>
    /// Стоимость товара
    /// </summary>
    public decimal ProductPrice { get; private set; }

    /// <summary>
    /// Товар
    /// </summary>
    public Product Product { get; private set; }

    /// <summary>
    /// Заказ
    /// </summary>
    public Order Order { get; private set; }

    private OrderItem() { }

    private OrderItem(int productId, int quantity, decimal productPrice, decimal itemTotalSum) 
    {
        ProductId = productId;
        Quantity = quantity;
        ProductPrice = productPrice;
        ItemTotalSum = productPrice * quantity;
    }

    private OrderItem(long id, int productId, int quantity, decimal productPrice, decimal itemTotalSum) 
    {
        Id = id;
        ProductId = productId;
        Quantity = quantity;
        ProductPrice = productPrice;
        ItemTotalSum = productPrice * quantity;
    }

    public static OrderItem CreateExisting(long id, int productId, int quantity, decimal productPrice, IStockInfo stockInfo)
    {
        Validate(productId, quantity, stockInfo);

        if (productPrice <= 0)
            throw new BusinessException(DomainErrors.Product.PricePositive());

        return new OrderItem(id, productId, quantity, productPrice, productPrice * quantity);
    }

    public void SetProduct(Product product)
    {
        Product = product;
    }

    /// <summary>
    /// Создать позицию заказа
    /// </summary>
    /// <param name="productId">Id товара</param>
    /// <param name="quantity">Количество товара</param>
    /// <param name="productPrice">Стоимость товара</param>
    /// <returns>Созданный элемент заказа</returns>
    public static OrderItem Create(int productId, int quantity, decimal productPrice, IStockInfo stockInfo)
    {
        Validate(productId, quantity, stockInfo);

        if (productPrice <= 0)
            throw new BusinessException(DomainErrors.Product.PricePositive());

        return new OrderItem(productId, quantity, productPrice, productPrice * quantity);
    }

    /// <summary>
    /// Обновить количество товара
    /// </summary>
    /// <param name="newQuantity">Новое количество товара</param>
    public void UpdateQuantity(int newQuantity, int productId, IStockInfo productStockInfo)
    {
        if (Quantity == newQuantity)
        {
            return;
        }

        Validate(productId, newQuantity, productStockInfo);

        Quantity = newQuantity;
        ItemTotalSum = ProductPrice * Quantity;
    }

    private static void Validate(int productId, int quantity, IStockInfo stockInfo)
    {
        if (productId <= 0)
            throw new BusinessException(DomainErrors.Validation.InvalidFormat(nameof(productId)));

        if (quantity <= 0)
            throw new BusinessException(DomainErrors.General.QuantityPositive());

        if (stockInfo == null || !stockInfo.IsStockQuantityAvailable(quantity))
            throw new BusinessException(DomainErrors.Product.StockNotAvailable(quantity, productId));
    }
}
