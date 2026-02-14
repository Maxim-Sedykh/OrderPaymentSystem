using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.UnitTests.Configurations.Factories.Builders;

public class ProductBuilder
{
    private int _id = 1;
    private string _name = "Test Product";
    private decimal _price = 100m;
    private int _stock = 10;

    public ProductBuilder WithId(int id) { _id = id; return this; }
    public ProductBuilder WithPrice(decimal price) { _price = price; return this; }
    public ProductBuilder WithStock(int stock) { _stock = stock; return this; }

    public Product Build() => Product.CreateExisting(_id, _name, "Desc", _price, _stock);
}
