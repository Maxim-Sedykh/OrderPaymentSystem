using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Extensions;
using OrderPaymentSystem.Domain.Interfaces.Entities;
using OrderPaymentSystem.Domain.Result;
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
    public DateTime CreatedAt { get; } = DateTime.Now;

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
    /// <returns>Результат создания заказа</returns>
    public static DataResult<Order> Create(
        Guid userId,
        Address deliveryAddress,
        ICollection<OrderItem> orderItems,
        decimal totalAmount)
    {
        if (deliveryAddress == null) return DataResult<Order>.Failure(9001, "Delivery address cannot be null.");
        if (orderItems.IsNotNullOrEmpty()) return DataResult<Order>.Failure(9002, "Order must contain at least one item.");
        if (totalAmount <= 0) return DataResult<Order>.Failure(9003, "Total amount must be positive.");

        var order = new Order
        {
            Id = default,
            UserId = userId,
            DeliveryAddress = deliveryAddress,
            TotalAmount = totalAmount,
            Status = OrderStatus.Pending,
            Items = orderItems
        };
        //TODO - надо как-то разобрваться с orderItems.Id

        return DataResult<Order>.Success(order);
    }

    /// <summary>
    /// Обновить статус заказа
    /// </summary>
    /// <param name="newStatus">Новый статус</param>
    /// <returns>Результат обновления статуса</returns>
    public BaseResult UpdateStatus(OrderStatus newStatus)
    {
        if (Status == OrderStatus.Delivered && newStatus != OrderStatus.Delivered && newStatus != OrderStatus.Refunded)
            return BaseResult.Failure(9004, "Cannot change status of a delivered order.");
        if (Status == OrderStatus.Cancelled && newStatus != OrderStatus.Cancelled)
            return BaseResult.Failure(9005, "Cannot change status of a cancelled order.");
        if (newStatus == Status)
            return BaseResult.Success();

        Status = newStatus;

        return BaseResult.Success();
    }

    /// <summary>
    /// Привязать платёж к текущему заказу
    /// </summary>
    /// <param name="paymentId">Id платежа</param>
    /// <returns>Результат привязки платежа</returns>
    public BaseResult AssignPayment(long paymentId)
    {
        if (PaymentId.HasValue) return BaseResult.Failure(9006, "Payment already assigned to this order.");
        PaymentId = paymentId;

        return BaseResult.Success();
    }

    /// <summary>
    /// Отменить заказ
    /// </summary>
    /// <returns>Результат отмены заказа</returns>
    public BaseResult CancelOrder()
    {
        if (Status == OrderStatus.Delivered || Status == OrderStatus.Shipped)
            return BaseResult.Failure(9007, "Cannot cancel an order that has already been shipped or delivered.");
        if (Status == OrderStatus.Cancelled)
            return BaseResult.Failure(9008, "Order is already cancelled.");

        return UpdateStatus(OrderStatus.Cancelled);
    }
}
