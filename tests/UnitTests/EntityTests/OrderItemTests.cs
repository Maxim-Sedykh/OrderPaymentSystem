using FluentAssertions;
using Moq;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Entities;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Shared.Exceptions;
using Xunit;

namespace OrderPaymentSystem.UnitTests.EntityTests;

public class OrderItemTests
{
    private readonly Mock<IStockInfo> _stockMock = new();

    [Fact]
    public void Create_ValidItem_ShouldCalculateTotalAndCheckStock()
    {
        // Arrange
        _stockMock.Setup(x => x.IsStockQuantityAvailable(5)).Returns(true);

        // Act
        var item = OrderItem.Create(1, 5, 100m, _stockMock.Object);

        // Assert
        item.ProductId.Should().Be(1);
        item.Quantity.Should().Be(5);
        item.ProductPrice.Should().Be(100m);
        item.ItemTotalSum.Should().Be(500m);
    }

    [Theory]
    [InlineData(0, 5, 100)]
    [InlineData(1, 0, 100)]
    [InlineData(1, 5, 0)]
    public void Create_WithInvalidData_ShouldThrowBusinessException(int productId, int quantity, decimal price)
    {
        // Arrange
        _stockMock.Setup(x => x.IsStockQuantityAvailable(It.IsAny<int>())).Returns(true);

        // Act
        Action act = () => OrderItem.Create(productId, quantity, price, _stockMock.Object);

        // Assert
        act.Should().Throw<BusinessException>();
    }

    [Fact]
    public void Create_WhenStockNotAvailable_ShouldThrowBusinessException()
    {
        // Arrange
        _stockMock.Setup(x => x.IsStockQuantityAvailable(5)).Returns(false);

        // Act
        Action act = () => OrderItem.Create(1, 5, 100m, _stockMock.Object);

        // Assert
        act.Should().Throw<BusinessException>();
    }

    [Fact]
    public void UpdateQuantity_WithValidNewQuantity_ShouldRecalculateTotal()
    {
        // Arrange
        _stockMock.Setup(x => x.IsStockQuantityAvailable(It.IsAny<int>())).Returns(true);
        var item = OrderItem.Create(1, 5, 100m, _stockMock.Object);

        // Act
        item.UpdateQuantity(10, 1, _stockMock.Object);

        // Assert
        item.Quantity.Should().Be(10);
        item.ItemTotalSum.Should().Be(1000m);
    }

    [Fact]
    public void UpdateQuantity_WhenStockNotAvailable_ShouldThrowBusinessException()
    {
        // Arrange
        var initialQuantity = 2;
        var newQuantity = 5;

        _stockMock.Setup(x => x.IsStockQuantityAvailable(initialQuantity)).Returns(true);
        var item = OrderItem.Create(1, initialQuantity, 100m, _stockMock.Object);

        _stockMock.Setup(x => x.IsStockQuantityAvailable(newQuantity)).Returns(false);

        // Act
        Action act = () => item.UpdateQuantity(5, 1, _stockMock.Object);

        // Assert
        act.Should().Throw<BusinessException>();
    }
}
