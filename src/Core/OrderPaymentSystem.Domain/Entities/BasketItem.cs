using OrderPaymentSystem.Domain.Abstract.Interfaces.Entities;
using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Shared.Exceptions;

namespace OrderPaymentSystem.Domain.Entities;

/// <summary>
/// Позиция в корзине пользователя. Намеревание пользователя приобрести товар
/// </summary>
public class BasketItem : IEntityId<long>, IAuditable
{
    /// <summary>
    /// Id позиции корзины
    /// </summary>
    public long Id { get; protected set; }

    /// <summary>
    /// Id владельца корзины - пользователя
    /// </summary>
    public Guid UserId { get; protected set; }

    /// <summary>
    /// Id товара
    /// </summary>
    public int ProductId { get; protected set; }

    /// <summary>
    /// Количество товара
    /// </summary>
    public int Quantity { get; private set; }

    /// <inheritdoc/>
    public DateTime CreatedAt { get; private set; }

    /// <inheritdoc/>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Владелец корзины - пользователь
    /// </summary>
    public User User { get; protected set; }

    /// <summary>
    /// Товар
    /// </summary>
    public Product Product { get; protected set; }

    protected BasketItem() { }

    /// <summary>
    /// Создать элемент корзины
    /// </summary>
    /// <param name="userId">Id пользователя</param>
    /// <param name="productId">Id товара</param>
    /// <param name="quantity">Количество товара</param>
    /// <param name="stockInfo">Информация о наличии товара на складе</param>
    /// <returns>Созданный элемент корзины</returns>
    public static BasketItem Create(Guid userId, int productId, int quantity, IStockInfo stockInfo)
    {
        if (userId == Guid.Empty)
            throw new BusinessException(DomainErrors.Validation.Required(nameof(userId)));

        if (productId <= 0)
            throw new BusinessException(DomainErrors.Validation.InvalidFormat(nameof(productId)));

        Validate(stockInfo, quantity, productId);

        return new BasketItem
        {
            UserId = userId,
            ProductId = productId,
            Quantity = quantity
        };
    }

    /// <summary>
    /// Обновить количество товара
    /// </summary>
    /// <param name="newQuantity">Новое количество товара</param>
    /// <param name="productId">Id товара</param>
    /// <param name="stockInfo">Информация о наличии товара на складе</param>
    public void UpdateQuantity(int newQuantity, int productId, IStockInfo stockInfo)
    {
        if (Quantity == newQuantity)
        {
            return;
        }

        Validate(stockInfo, newQuantity, productId);

        Quantity = newQuantity;
    }

    public static void Validate(IStockInfo stockInfo, int quantity, int productId)
    {
        if (!stockInfo.IsStockQuantityAvailable(quantity))
            throw new BusinessException(DomainErrors.Product.StockNotAvailable(quantity, productId));

        if (quantity <= 0)
            throw new BusinessException(DomainErrors.General.QuantityPositive());
    }
}