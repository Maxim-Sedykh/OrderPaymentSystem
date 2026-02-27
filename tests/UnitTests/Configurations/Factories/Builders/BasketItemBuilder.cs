using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.UnitTests.Configurations.Factories.Builders;

/// <summary>
/// Билдер для построения мокового элемента корзины
/// </summary>
public class BasketItemBuilder
{
    private long _id = 1;
    private Guid _userId = Guid.NewGuid();
    private int _productId = 1;
    private int _quantity = 1;
    private Product _product = TestDataFactory.Product.Build();

    /// <summary>
    /// Добавить количество
    /// </summary>
    public BasketItemBuilder WithQuantity(int quantity) { _quantity = quantity; return this; }

    /// <summary>
    /// Построить объект
    /// </summary>
    /// <returns></returns>
    public BasketItem Build()
    {
        var basketItem = BasketItem.CreateExisting(_id, _userId, _productId, _quantity, _product);

        basketItem.SetProduct(_product);

        return basketItem;
    }
}
