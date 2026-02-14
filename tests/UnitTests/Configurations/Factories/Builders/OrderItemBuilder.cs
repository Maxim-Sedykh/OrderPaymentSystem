using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.UnitTests.Configurations.Factories.Builders;

public class OrderItemBuilder
{
    private long _id = 1;
    private int _productId = 1;
    private int _quantity = 1;
    private decimal _price = 100m;
    private Product _product = TestDataFactory.Product.Build();

    public OrderItemBuilder WithId(long id) { _id = id; return this; }
    public OrderItemBuilder WithProduct(Product product)
    {
        _product = product;
        _productId = product.Id;
        _price = product.Price;
        return this;
    }
    public OrderItemBuilder WithQuantity(int quantity) { _quantity = quantity; return this; }

    public OrderItem Build()
    {
        var orderItem = OrderItem.CreateExisting(_id, _productId, _quantity, _price, _product);

        orderItem.SetProduct(_product);

        return orderItem;
    }
}
