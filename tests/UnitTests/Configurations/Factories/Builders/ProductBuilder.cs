using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.UnitTests.Configurations.Factories.Builders;

/// <summary>
/// Билдер для построения мокового товара
/// </summary>
public class ProductBuilder
{
    private int _id = 1;
    private string _name = "Test Product";
    private decimal _price = 100m;
    private int _stock = 10;

    /// <summary>
    /// Добавить Id
    /// </summary>
    public ProductBuilder WithId(int id) { _id = id; return this; }

    /// <summary>
    /// Добавить цену товару
    /// </summary>
    public ProductBuilder WithPrice(decimal price) { _price = price; return this; }

    /// <summary>
    /// Добавить количество товара на складе
    /// </summary>
    public ProductBuilder WithStock(int stock) { _stock = stock; return this; }

    /// <summary>
    /// Построить, создать объект.
    /// </summary>
    /// <returns>Созданный товар</returns>
    public Product Build() => Product.CreateExisting(_id, _name, "Desc", _price, _stock);
}
