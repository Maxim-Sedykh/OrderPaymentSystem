using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.ValueObjects;

namespace OrderPaymentSystem.UnitTests.Configurations.Factories.Builders;

/// <summary>
/// Билдер для построения мокового заказа
/// </summary>
public class OrderBuilder
{
    private long _id = 1;
    private Guid _userId = Guid.NewGuid();
    private OrderStatus _status = OrderStatus.Pending;
    private List<OrderItem> _items = new();
    private Address _address = new("Default St", "City", "123", "Country");

    /// <summary>
    /// Добавить статус
    /// </summary>
    public OrderBuilder WithStatus(OrderStatus status) { _status = status; return this; }

    /// <summary>
    /// Добавиь элементы заказа
    /// </summary>
    public OrderBuilder WithItems(params OrderItem[] items) { _items.AddRange(items); return this; }

    /// <summary>
    /// Построить, создать объект.
    /// </summary>
    /// <returns>Созданный заказ</returns>
    public Order Build()
    {
        var order = Order.CreateExisting(_id, _userId, _address, _items, _items.Sum(i => i.ItemTotalSum), _status);
        return order;
    }
}
