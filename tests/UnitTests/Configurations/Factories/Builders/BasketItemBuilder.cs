using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.UnitTests.Configurations.Factories.Builders;

public class BasketItemBuilder
{
    private long _id = 1;
    private Guid _userId = Guid.NewGuid();
    private int _productId = 1;
    private int _quantity = 1;
    private Product _product = TestDataFactory.Product.Build();

    public BasketItemBuilder WithId(long id) { _id = id; return this; }
    public BasketItemBuilder WithUser(Guid userId) { _userId = userId; return this; }
    public BasketItemBuilder WithProduct(Product product) { _product = product; _productId = product.Id; return this; }
    public BasketItemBuilder WithQuantity(int quantity) { _quantity = quantity; return this; }

    public BasketItem Build()
    {
        var basketItem = BasketItem.CreateExisting(_id, _userId, _productId, _quantity, _product);

        basketItem.SetProduct(_product);

        return basketItem;
    }
}
