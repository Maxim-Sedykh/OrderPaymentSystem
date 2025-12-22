using OrderPaymentSystem.Domain.Exceptions;
using OrderPaymentSystem.Domain.Interfaces.Entities;
using OrderPaymentSystem.Domain.Result;

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
    public int Quantity { get; protected set; }

    /// <inheritdoc/>
    public DateTime CreatedAt { get; }

    /// <inheritdoc/>
    public DateTime? UpdatedAt { get; }

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
    /// <returns>Созданная корзина</returns>
    public static BasketItem Create(Guid userId, int productId, int quantity)
    {
        if (quantity <= 0)
            throw new BusinessException(3001, "Quantity must be positive.");

        return DataResult<BasketItem>.Success(new BasketItem
        {
            Id = default,
            UserId = userId,
            ProductId = productId,
            Quantity = quantity
        });
    }

    /// <summary>
    /// Обновить количество товара
    /// </summary>
    /// <param name="newQuantity">Новое количество товара</param>
    /// <returns>Результат обновления</returns>
    public BaseResult UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0) 
            return BaseResult.Failure(3002, "Quantity must be positive.");

        Quantity = newQuantity;

        return BaseResult.Success();
    }
}