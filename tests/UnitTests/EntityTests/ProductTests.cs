using FluentAssertions;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Shared.Exceptions;

namespace OrderPaymentSystem.UnitTests.EntityTests;

/// <summary>
/// Тесты сущности <see cref="Product"/>
/// </summary>
public class ProductTests
{
    /// <summary>
    /// Создание товара с валидными данными должно быть успешно
    /// </summary>
    [Fact]
    public void Create_WithValidData_ShouldReturnProduct()
    {
        // Act
        var product = Product.Create("Laptop", "Description", 1000m, 10);

        // Assert
        product.Name.Should().Be("Laptop");
        product.Price.Should().Be(1000m);
        product.StockQuantity.Should().Be(10);
    }

    /// <summary>
    /// Создание товара с инвалидными данными должно выбрасывать BusinessException
    /// </summary>
    [Theory]
    [InlineData("", 100)]
    [InlineData("Name", 0)]
    [InlineData("Name", -1)]
    public void Create_WithInvalidData_ShouldThrowBusinessException(string name, decimal price)
    {
        // Act
        Action act = () => Product.Create(name, "Desc", price, 10);

        // Assert
        act.Should().Throw<BusinessException>();
    }

    /// <summary>
    /// Уменьшение товара на складе должно уменьшать поле StockQuantity
    /// </summary>
    [Fact]
    public void ReduceStockQuantity_WhenStockIsEnough_ShouldDecreaseQuantity()
    {
        // Arrange
        var product = Product.Create("Test", "Desc", 100m, 10);

        // Act
        product.ReduceStockQuantity(4);

        // Assert
        product.StockQuantity.Should().Be(6);
    }

    /// <summary>
    /// Уменьшение товара на складе на число, которого нет на складе - должно выбрасывать BusinessException
    /// </summary>
    [Fact]
    public void ReduceStockQuantity_WhenStockIsNotEnough_ShouldThrowBusinessException()
    {
        // Arrange
        var product = Product.Create("Test", "Desc", 100m, 5);

        // Act
        Action act = () => product.ReduceStockQuantity(10);

        // Assert
        act.Should().Throw<BusinessException>();
    }
}
