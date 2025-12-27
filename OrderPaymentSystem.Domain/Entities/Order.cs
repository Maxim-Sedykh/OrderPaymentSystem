using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Exceptions;
using OrderPaymentSystem.Domain.Extensions;
using OrderPaymentSystem.Domain.Interfaces.Entities;
using OrderPaymentSystem.Domain.ValueObjects;

namespace OrderPaymentSystem.Domain.Entities;

/// <summary>
/// Заказ пользователя
/// </summary>
public class Order : IEntityId<long>, IAuditable
{
    /// <summary>
    /// Id заказа
    /// </summary>
    public long Id { get; protected set; }

    /// <summary>
    /// Id пользователя который совершает заказ
    /// </summary>
    public Guid UserId { get; protected set; }

    /// <summary>
    /// Id платежа по заказу
    /// </summary>
    public long? PaymentId { get; protected set; }

    /// <summary>
    /// Общая стоимость заказа
    /// </summary>
    public decimal TotalAmount { get; protected set; }

    /// <summary>
    /// Статус заказа
    /// </summary>
    public OrderStatus Status { get; protected set; }

    /// <summary>
    /// Адрес доставки заказа
    /// </summary>
    public Address DeliveryAddress { get; protected set; }

    /// <inheritdoc/>
    public DateTime CreatedAt { get; }

    /// <inheritdoc/>
    public DateTime? UpdatedAt { get; }

    /// <summary>
    /// Элементы заказа
    /// </summary>
    public ICollection<OrderItem> Items { get; protected set; } = [];

    /// <summary>
    /// Платёж по заказу
    /// </summary>
    public Payment Payment { get; protected set; }

    /// <summary>
    /// Пользователь который совершает заказ
    /// </summary>
    public User User { get; protected set; }

    protected Order() { }

    /// <summary>
    /// Создать заказ
    /// </summary>
    /// <param name="userId">Id пользователя</param>
    /// <param name="deliveryAddress">Адрес доставки заказа</param>
    /// <param name="orderItems">Элементы заказа</param>
    /// <param name="totalAmount">Общая стоимость заказа</param>
    /// <returns>Созданный заказ</returns>
    public static Order Create(
        Guid userId,
        Address deliveryAddress,
        ICollection<OrderItem> orderItems,
        decimal totalAmount)
    {
        if (userId == Guid.Empty)
            throw new BusinessException(1001, "User ID cannot be empty.");

        if (deliveryAddress == null)
            throw new BusinessException(9001, "Delivery address cannot be null.");

        if (orderItems.IsNotNullOrEmpty())
            throw new BusinessException(9002, "Order must contain at least one item.");

        if (totalAmount <= 0)
            throw new BusinessException(9003, "Total amount must be positive.");

        var order = new Order
        {
            Id = default,
            UserId = userId,
            DeliveryAddress = deliveryAddress,
            TotalAmount = totalAmount,
            Status = OrderStatus.Pending,
            Items = orderItems
        };

        return order;
    }

    /// <summary>
    /// Обновить статус заказа
    /// </summary>
    /// <param name="newStatus">Новый статус</param>
    public void UpdateStatus(OrderStatus newStatus)
    {
        if (Status == OrderStatus.Delivered && newStatus != OrderStatus.Delivered && newStatus != OrderStatus.Refunded)
            throw new BusinessException(9004, "Cannot change status of a delivered order.");
        if (Status == OrderStatus.Cancelled && newStatus != OrderStatus.Cancelled)
            throw new BusinessException(9005, "Cannot change status of a cancelled order.");

        Status = newStatus;
    }

    /// <summary>
    /// Привязать платёж к текущему заказу
    /// </summary>
    /// <param name="paymentId">Id платежа</param>
    public void AssignPayment(long paymentId)
    {
        if (paymentId <= 0)
            throw new BusinessException(1001, "Payment ID must be positive.");

        if (PaymentId.HasValue)
            throw new BusinessException(9006, "Payment already assigned to this order.");

        PaymentId = paymentId;
    }

    /// <summary>
    /// Отметить заказ как отправленный (Shipped).
    /// </summary>
    public void ShipOrder()
    {
        if (Status != OrderStatus.Confirmed)
            throw new BusinessException(666, $"Order must be 'Confirmed' to be shipped. Current status: {Status}.");
        if (!Items.IsNotNullOrEmpty())
            throw new BusinessException(666, "Cannot ship an empty order.");
        if (!PaymentId.HasValue)
            throw new BusinessException(666, "Order cannot be shipped without a payment.");

        UpdateStatus(OrderStatus.Shipped);
    }

    /// <summary>
    /// Подтвердить заказ. Например, после проверки доступности товаров или оплаты.
    /// </summary>
    public void ConfirmOrder()
    {
        if (Status != OrderStatus.Pending)
            throw new BusinessException(666, $"Order status is {Status}, cannot confirm.");
        if (!Items.Any())
            throw new BusinessException(666, "Cannot confirm an empty order.");
        if (!PaymentId.HasValue)
            throw new BusinessException(666, "Order cannot be confirmed without an assigned payment.");

        UpdateStatus(OrderStatus.Confirmed);
    }

    /// <summary>
    /// Корректирует количество существующей позиции заказа или добавляет новую.
    /// Этот метод более высокоуровневый и может использоваться для массовых обновлений.
    /// </summary>
    /// <param name="productId">ID продукта</param>
    /// <param name="quantityChange">Изменение количества (+ для добавления, - для уменьшения)</param>
    /// <param name="productPrice">Текущая цена продукта (для новых позиций)</param>
    public void UpdateOrderItems(int productId, int quantityChange, decimal productPrice)
    {
        if (Status != OrderStatus.Pending && Status != OrderStatus.Confirmed)
            throw new BusinessException(666, "OrderCannotAddOrRemoveItemInCurrentStatus");

        var existingItem = Items.FirstOrDefault(x => x.ProductId == productId);

        if (existingItem != null)
        {
            var newQuantity = existingItem.Quantity + quantityChange;
            if (newQuantity <= 0)
            {
                Items.Remove(existingItem);
            }
            else
            {
                existingItem.UpdateQuantity(newQuantity);
            }
        }
        else
        {
            // Если позиция не существует и мы пытаемся добавить
            if (quantityChange > 0)
            {
                if (productPrice <= 0)
                    throw new BusinessException(666, "OrderItemPriceInvalid");

                var newItem = OrderItem.Create(productId, quantityChange, productPrice);
                Items.Add(newItem);
            }
            else
            {
                // Попытка уменьшить количество несуществующего товара
                throw new BusinessException(666, "OrderCannotRemoveNonExistingItem");
            }
        }
        RecalculateTotalAmount();
    }
}
