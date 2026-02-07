using FluentAssertions;
using Moq;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Entities;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Shared.Exceptions;
using Xunit;

namespace OrderPaymentSystem.UnitTests.EntityTests;

public class BasketItemTests
{
    private readonly Mock<IStockInfo> _stockMock = new();

    [Fact]
    public void Create_ValidItem_ShouldSetProperties()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = 1;
        var quantity = 5;
        _stockMock.Setup(x => x.IsStockQuantityAvailable(quantity)).Returns(true);

        // Act
        var basketItem = BasketItem.Create(userId, productId, quantity, _stockMock.Object);

        // Assert
        basketItem.UserId.Should().Be(userId);
        basketItem.ProductId.Should().Be(productId);
        basketItem.Quantity.Should().Be(quantity);
        basketItem.Id.Should().Be(default);
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000", 1, 5)]
    [InlineData("b2d7b42a-a9a2-4a7b-a4b2-2b8b9b8b9b8b", 0, 5)]
    [InlineData("b2d7b42a-a9a2-4a7b-a4b2-2b8b9b8b9b8b", 1, 0)]
    public void Create_WithInvalidData_ShouldThrowBusinessException(string userIdString, int productId, int quantity)
    {
        // Arrange
        var userId = Guid.Parse(userIdString);
        _stockMock.Setup(x => x.IsStockQuantityAvailable(It.IsAny<int>())).Returns(true);

        // Act
        Action act = () => BasketItem.Create(userId, productId, quantity, _stockMock.Object);

        // Assert
        act.Should().Throw<BusinessException>();
    }

    [Fact]
    public void Create_WhenStockNotAvailable_ShouldThrowBusinessException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = 1;
        var quantity = 5;
        _stockMock.Setup(x => x.IsStockQuantityAvailable(quantity)).Returns(false);

        // Act
        Action act = () => BasketItem.Create(userId, productId, quantity, _stockMock.Object);

        // Assert
        act.Should().Throw<BusinessException>();
    }

    [Fact]
    public void UpdateQuantity_WithValidNewQuantity_ShouldUpdateQuantity()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = 1;
        var initialQuantity = 5;
        var newQuantity = 10;
        _stockMock.Setup(x => x.IsStockQuantityAvailable(initialQuantity)).Returns(true);
        _stockMock.Setup(x => x.IsStockQuantityAvailable(newQuantity)).Returns(true);
        var basketItem = BasketItem.Create(userId, productId, initialQuantity, _stockMock.Object);

        // Act
        basketItem.UpdateQuantity(newQuantity, productId, _stockMock.Object);

        // Assert
        basketItem.Quantity.Should().Be(newQuantity);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void UpdateQuantity_WithInvalidNewQuantity_ShouldThrowBusinessException(int newQuantity)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = 1;
        _stockMock.Setup(x => x.IsStockQuantityAvailable(It.IsAny<int>())).Returns(true);
        var basketItem = BasketItem.Create(userId, productId, 5, _stockMock.Object);

        // Act
        Action act = () => basketItem.UpdateQuantity(newQuantity, productId, _stockMock.Object);

        // Assert
        act.Should().Throw<BusinessException>();
    }

    [Fact]
    public void UpdateQuantity_WhenNewQuantityNotAvailable_ShouldThrowBusinessException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = 1;
        _stockMock.Setup(x => x.IsStockQuantityAvailable(5)).Returns(true);
        var basketItem = BasketItem.Create(userId, productId, 5, _stockMock.Object);

        _stockMock.Setup(x => x.IsStockQuantityAvailable(10)).Returns(false);

        // Act
        Action act = () => basketItem.UpdateQuantity(10, productId, _stockMock.Object);

        // Assert
        act.Should().Throw<BusinessException>();
    }
}
