using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.ValueObjects;

namespace OrderPaymentSystem.UnitTests.Configurations.Factories.Builders;

public class OrderBuilder
{
    private long _id = 1;
    private Guid _userId = Guid.NewGuid();
    private OrderStatus _status = OrderStatus.Pending;
    private List<OrderItem> _items = new();
    private Address _address = new("Default St", "City", "123", "Country");

    public OrderBuilder WithId(long id) { _id = id; return this; }
    public OrderBuilder WithStatus(OrderStatus status) { _status = status; return this; }
    public OrderBuilder WithItems(params OrderItem[] items) { _items.AddRange(items); return this; }

    public Order Build()
    {
        var order = Order.CreateExisting(_id, _userId, _address, _items, _items.Sum(i => i.ItemTotalSum), _status);
        return order;
    }
}
