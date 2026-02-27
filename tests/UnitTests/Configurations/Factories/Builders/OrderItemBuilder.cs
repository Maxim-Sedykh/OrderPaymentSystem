using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.UnitTests.Configurations.Factories.Builders;

/// <summary>
/// Билдер для построения мокового элемента заказа
/// </summary>
public class OrderItemBuilder
{
    private long _id = 1;
    private int _productId = 1;
    private int _quantity = 1;
    private decimal _price = 100m;
    private Product _product = TestDataFactory.Product.Build();

    /// <summary>
    /// Добавить товар
    /// </summary>
    public OrderItemBuilder WithProduct(Product product)
    {
        _product = product;
        _productId = product.Id;
        _price = product.Price;
        return this;
    }

    /// <summary>
    /// Добавить количество
    /// </summary>
    public OrderItemBuilder WithQuantity(int quantity) { _quantity = quantity; return this; }

    /// <summary>
    /// Построить, создать объект.
    /// </summary>
    /// <returns>Созданный элемент заказа</returns>
    public OrderItem Build()
    {
        var orderItem = OrderItem.CreateExisting(_id, _productId, _quantity, _price, _product);

        orderItem.SetProduct(_product);

        return orderItem;
    }
}
