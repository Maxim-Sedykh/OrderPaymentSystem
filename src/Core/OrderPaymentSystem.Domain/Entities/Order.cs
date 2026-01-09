using OrderPaymentSystem.Domain.Constants;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces.Entities;
using OrderPaymentSystem.Domain.ValueObjects;
using OrderPaymentSystem.Shared.Exceptions;
using OrderPaymentSystem.Shared.Extensions;

namespace OrderPaymentSystem.Domain.Entities;

/// <summary>
/// Заказ пользователя
/// </summary>
public class Order : IEntityId<long>, IAuditable
{
    /// <summary>
    /// Элементы заказа. Внутренняя коллекция элементов
    /// </summary>
    private readonly List<OrderItem> _items = [];

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
    /// Элементы заказа. Для доступа из внешнего кода
    /// </summary>
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

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
    /// <param name="address">Адрес доставки заказа</param>
    /// <param name="items">Элементы заказа</param>
    /// <returns>Созданный заказ</returns>
    public static Order Create(Guid userId, Address address, IEnumerable<OrderItem> items)
    {
        if (userId == Guid.Empty) throw new BusinessException(ErrorCodes.InvalidUserId, "User ID empty");
        if (address == null) throw new BusinessException(ErrorCodes.OrderDeliveryAddressRequired, "Address null");

        var itemList = items?.ToList();
        if (itemList.IsNullOrEmpty())
            throw new BusinessException(ErrorCodes.OrderItemsNotFound, "Order items empty");

        var order = new Order
        {
            UserId = userId,
            DeliveryAddress = address,
            Status = OrderStatus.Pending
        };

        foreach (var item in itemList) order._items.Add(item);

        order.RecalculateTotalAmount();
        return order;
    }

    /// <summary>
    /// Обновить статус заказа
    /// </summary>
    /// <param name="newStatus">Новый статус</param>
    public void UpdateStatus(OrderStatus newStatus)
    {
        if (Status == OrderStatus.Delivered && newStatus != OrderStatus.Delivered && newStatus != OrderStatus.Refunded)
            throw new BusinessException(ErrorCodes.OrderCannotBeInStatusWhenDelivered, "Cannot change status of a delivered order.");
        if (Status == OrderStatus.Cancelled && newStatus != OrderStatus.Cancelled)
            throw new BusinessException(ErrorCodes.OrderCannotChangeStatusOfACancelledOrder, "Cannot change status of a cancelled order.");

        Status = newStatus;
    }

    /// <summary>
    /// Привязать платёж к текущему заказу
    /// </summary>
    /// <param name="paymentId">Id платежа</param>
    public void AssignPayment(long paymentId)
    {
        if (paymentId <= 0)
            throw new BusinessException(ErrorCodes.InvalidPaymentId, "Payment ID must be positive.");

        if (PaymentId.HasValue)
            throw new BusinessException(ErrorCodes.PaymentAlreadyExistsForOrder, "Payment already assigned to this order.");

        PaymentId = paymentId;
    }

    /// <summary>
    /// Отметить заказ как отправленный
    /// </summary>
    public void ShipOrder()
    {
        if (Status != OrderStatus.Confirmed)
            throw new BusinessException(ErrorCodes.OrderStatusChangeNotAllowed, $"Order must be 'Confirmed' to be shipped. Current status: {Status}.");
        if (Items.IsNullOrEmpty())
            throw new BusinessException(ErrorCodes.OrderItemsNotFound, "Cannot ship an empty order.");
        if (!PaymentId.HasValue)
            throw new BusinessException(ErrorCodes.PaymentNotFound, "Order cannot be shipped without a payment.");

        UpdateStatus(OrderStatus.Shipped);
    }

    /// <summary>
    /// Подтвердить заказ
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
    /// <param name="stockInfo">Информация о наличии товара на складе</param>
    public void UpdateOrderItem(int productId, int quantityChange, decimal productPrice, IStockInfo stockInfo)
    {
        if (Status != OrderStatus.Pending && Status != OrderStatus.Confirmed)
            throw new BusinessException(ErrorCodes.OrderCannotAddOrRemoveItemInCurrentStatus, "OrderCannotAddOrRemoveItemInCurrentStatus");

        var existingItem = _items.FirstOrDefault(x => x.ProductId == productId);

        if (existingItem != null)
        {
            var newQuantity = existingItem.Quantity + quantityChange;
            if (newQuantity <= 0)
            {
                _items.Remove(existingItem);
            }
            else
            {
                existingItem.UpdateQuantity(newQuantity, stockInfo);
            }
        }
        else
        {
            if (quantityChange > 0)
            {
                if (productPrice <= 0)
                    throw new BusinessException(ErrorCodes.OrderItemPriceInvalid, "OrderItemPriceInvalid");

                var newItem = OrderItem.Create(productId, quantityChange, productPrice, stockInfo);
                _items.Add(newItem);
            }
            else
            {
                throw new BusinessException(ErrorCodes.OrderCannotRemoveNonExistingItem, "OrderCannotRemoveNonExistingItem");
            }
        }
        RecalculateTotalAmount();
    }

    public void AddOrderItem(OrderItem item, IStockInfo stockInfo)
    {
        if (!stockInfo.IsStockQuantityAvailable(item.Quantity))
            throw new BusinessException(ErrorCodes.ProductStockQuantityNotAvailable, "ProductStockQuantityNotAvailable");

        _items.Add(item);

        RecalculateTotalAmount();
    }

    public void RemoveOrderItem(OrderItem item)
    {
        _items.Remove(item);

        RecalculateTotalAmount();
    }

    public void UpdateOrderItemQuantity(long orderItemId, int newQuantity, IStockInfo stockInfo)
    {
        var orderItem = _items.FirstOrDefault(x => x.Id == orderItemId);
        orderItem.UpdateQuantity(newQuantity, stockInfo);

        RecalculateTotalAmount();
    }

    /// <summary>
    /// Пересчитывает общую сумму заказа на основе стоимости его позиций.
    /// </summary>
    public void RecalculateTotalAmount()
    {
        if (_items == null || !_items.Any())
        {
            TotalAmount = 0;
            return;
        }

        TotalAmount = _items.Sum(item => item.Quantity * item.ProductPrice);
    }
}
