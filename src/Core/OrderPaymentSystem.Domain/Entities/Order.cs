using OrderPaymentSystem.Domain.Abstract;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Entities;
using OrderPaymentSystem.Domain.Constants;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Domain.ValueObjects;
using OrderPaymentSystem.Shared.Exceptions;
using OrderPaymentSystem.Shared.Extensions;

namespace OrderPaymentSystem.Domain.Entities;

/// <summary>
/// Заказ пользователя
/// </summary>
public class Order : BaseEntity<long>, IAuditable
{
    /// <summary>
    /// Элементы заказа. Внутренняя коллекция элементов
    /// </summary>
    private readonly List<OrderItem> _items = [];

    /// <summary>
    /// Id пользователя который совершает заказ
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Id платежа по заказу
    /// </summary>
    public long? PaymentId { get; private set; }

    /// <summary>
    /// Общая стоимость заказа
    /// </summary>
    public decimal TotalAmount { get; private set; }

    /// <summary>
    /// Статус заказа
    /// </summary>
    public OrderStatus Status { get; private set; }

    /// <summary>
    /// Адрес доставки заказа
    /// </summary>
    public Address? DeliveryAddress { get; private set; }

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
    public Payment? Payment { get; private set; }

    /// <summary>
    /// Пользователь который совершает заказ
    /// </summary>
    public User? User { get; private set; }

    private Order() { }

    private Order(Guid userId, Address address, OrderStatus status) 
    {
        UserId = userId;
        DeliveryAddress = address;
        Status = status;
    }

    private Order(long id, Guid userId, Address address, OrderStatus status, decimal totalAmountSum) 
    {
        Id = id;
        UserId = userId;
        DeliveryAddress = address;
        Status = status;
        TotalAmount = totalAmountSum;
    }

    /// <summary>
    /// Назначить платёж. Используется только для тестов.
    /// </summary>
    /// <param name="payment"></param>
    internal void SetPayment(Payment payment)
    {
        Payment = payment;
    }

    /// <summary>
    /// Создать существующий заказ. Используется только для тестов
    /// </summary>
    internal static Order CreateExisting(long id,
        Guid userId,
        Address address,
        IEnumerable<OrderItem> items,
        decimal totalAmount,
        OrderStatus status)
    {
        ValidateCreate(userId, address, items);

        var order = new Order(id, userId, address, status, totalAmount);

        order._items.AddRange(items);

        return order;
    }

    /// <summary>
    /// Создать заказ
    /// </summary>
    /// <param name="userId">Id пользователя</param>
    /// <param name="address">Адрес доставки заказа</param>
    /// <param name="items">Элементы заказа</param>
    /// <returns>Созданный заказ</returns>
    public static Order Create(Guid userId, Address address, IEnumerable<OrderItem> items)
    {
        ValidateCreate(userId, address, items);

        var order = new Order(userId, address, OrderStatus.Pending);

        order._items.AddRange(items);

        order.RecalculateTotalAmount();
        return order;
    }

    /// <summary>
    /// Обновить статус заказа
    /// </summary>
    /// <param name="newStatus">Новый статус</param>
    public void UpdateStatus(OrderStatus newStatus)
    {
        if (newStatus == Status)
        {
            return;
        }

        if ((Status == OrderStatus.Delivered 
            && newStatus != OrderStatus.Delivered 
            && newStatus != OrderStatus.Refunded)
            || (Status == OrderStatus.Cancelled 
                && newStatus != OrderStatus.Cancelled))
            throw new BusinessException(DomainErrors.Order.StatusChangeNotAllowed(Status.ToString(), newStatus.ToString()));

        Status = newStatus;
    }

    /// <summary>
    /// Привязать платёж к текущему заказу
    /// </summary>
    /// <param name="paymentId">Id платежа</param>
    public void AssignPayment(long paymentId)
    {
        if (paymentId <= 0)
            throw new BusinessException(DomainErrors.Validation.InvalidFormat(nameof(paymentId)));

        if (PaymentId.HasValue)
            throw new BusinessException(DomainErrors.Payment.AlreadyExists(Id));

        PaymentId = paymentId;
    }

    /// <summary>
    /// Отметить заказ как отправленный
    /// </summary>
    public void ShipOrder()
    {
        if (Status != OrderStatus.Confirmed)
            throw new BusinessException(DomainErrors.Order.StatusChangeNotAllowed(Status.ToString(), OrderStatus.Shipped.ToString()));
        EnsureCanProcess();

        UpdateStatus(OrderStatus.Shipped);
    }

    /// <summary>
    /// Подтвердить заказ
    /// </summary>
    public void ConfirmOrder()
    {
        if (Status != OrderStatus.Pending)
            throw new BusinessException(DomainErrors.Order.StatusChangeNotAllowed(Status.ToString(), OrderStatus.Confirmed.ToString()));
        EnsureCanProcess();

        UpdateStatus(OrderStatus.Confirmed);
    }

    /// <summary>
    /// Корректирует количество существующей позиции заказа или добавляет новую.
    /// </summary>
    /// <param name="productId">ID продукта</param>
    /// <param name="quantityChange">Изменение количества (+ для добавления, - для уменьшения)</param>
    /// <param name="productPrice">Текущая цена продукта (для новых позиций)</param>
    /// <param name="stockInfo">Информация о наличии товара на складе</param>
    public void UpdateOrderItem(int productId, int quantityChange, decimal productPrice, IStockInfo stockInfo)
    {
        if (Status != OrderStatus.Pending && Status != OrderStatus.Confirmed)
            throw new BusinessException(DomainErrors.Order.CannotAddOrRemoveItemInCurrentStatus(Status));

        var existingItem = _items.FirstOrDefault(x => x.ProductId == productId);

        if (existingItem is not null)
        {
            var newQuantity = existingItem.Quantity + quantityChange;
            if (newQuantity <= 0)
            {
                _items.Remove(existingItem);
            }
            else
            {
                existingItem.UpdateQuantity(newQuantity, productId, stockInfo);
            }
        }
        else if (quantityChange > 0)
        {
            if (productPrice <= 0)
                throw new BusinessException(DomainErrors.Product.PricePositive());

            var newItem = OrderItem.Create(productId, quantityChange, productPrice, stockInfo);
            _items.Add(newItem);
        }
        else
        {
            throw new BusinessException(ErrorCodes.OrderCannotRemoveNonExistingItem, "OrderCannotRemoveNonExistingItem");
        }

        RecalculateTotalAmount();
    }
    
    /// <summary>
    /// Добавить элемент к заказу
    /// </summary>
    /// <param name="item">Элемент заказа</param>
    /// <param name="stockInfo">Количество на складе</param>
    /// <exception cref="BusinessException"></exception>
    public void AddOrderItem(OrderItem item, IStockInfo stockInfo)
    {
        if (!stockInfo.IsStockQuantityAvailable(item.Quantity))
            throw new BusinessException(DomainErrors.Product.StockNotAvailable(item.ProductId, item.Quantity));

        _items.Add(item);

        RecalculateTotalAmount();
    }

    /// <summary>
    /// Удалить элемент из заказа
    /// </summary>
    /// <param name="item">Элемент</param>
    public void RemoveOrderItem(OrderItem item)
    {
        _items.Remove(item);

        RecalculateTotalAmount();
    }

    /// <summary>
    /// Обновить количество в элементе заказа
    /// </summary>
    /// <param name="orderItemId">Id элемента заказа</param>
    /// <param name="newQuantity">Новое количество товара</param>
    /// <param name="stockInfo">Количество товара на складе</param>
    public void UpdateOrderItemQuantity(long orderItemId, int newQuantity, IStockInfo stockInfo)
    {
        var orderItem = _items.FirstOrDefault(x => x.Id == orderItemId) ?? throw new BusinessException(DomainErrors.Order.ItemNotFound(orderItemId));
        orderItem.UpdateQuantity(newQuantity, orderItem.ProductId, stockInfo);

        RecalculateTotalAmount();
    }

    /// <summary>
    /// Пересчитывает общую сумму заказа на основе стоимости его позиций.
    /// </summary>
    public void RecalculateTotalAmount()
    {
        if (_items == null || _items.Count == 0)
        {
            TotalAmount = 0;
            return;
        }

        TotalAmount = _items.Sum(item => item.Quantity * item.ProductPrice);
    }
    
    /// <summary>
    /// Валидировать заказ, чтобы далее взаимодействовать с ним
    /// </summary>
    /// <exception cref="BusinessException"></exception>
    private void EnsureCanProcess()
    {
        if (_items.IsNullOrEmpty()) throw new BusinessException(DomainErrors.Order.ItemsEmpty());
        if (!PaymentId.HasValue) throw new BusinessException(DomainErrors.Validation.Required(nameof(PaymentId)));
    }

    /// <summary>
    /// Валидировать создание заказа
    /// </summary>
    /// <param name="userId">Id пользователя который создал заказ</param>
    /// <param name="address">Адрес доставки</param>
    /// <param name="items">Элементы заказа</param>
    /// <exception cref="BusinessException"></exception>
    private static void ValidateCreate(Guid userId, Address address, IEnumerable<OrderItem> items)
    {
        if (userId == Guid.Empty) throw new BusinessException(DomainErrors.Validation.InvalidFormat(nameof(userId)));
        if (address == null) throw new BusinessException(DomainErrors.Order.DeliveryAddressRequired());

        var itemList = items?.ToList();
        if (itemList!.IsNullOrEmpty())
            throw new BusinessException(DomainErrors.Order.ItemsEmpty());
    }
}
