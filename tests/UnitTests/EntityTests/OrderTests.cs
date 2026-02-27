using FluentAssertions;
using Moq;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Entities;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.ValueObjects;
using OrderPaymentSystem.Shared.Exceptions;

namespace OrderPaymentSystem.UnitTests.EntityTests;

/// <summary>
/// Тесты сущности <see cref="Order"/>
/// </summary>
public class OrderTests
{
    private readonly Mock<IStockInfo> _stockMock = new();

    /// <summary>
    /// Создание заказа с валидными данными должно быть успешно.
    /// Заказ должен иметь статус Pending, и правильно расчитанную TotalAmount
    /// </summary>
    [Fact]
    public void Create_ValidOrder_ShouldHavePendingStatusAndCorrectTotal()
    {
        // Arrange
        _stockMock.Setup(x => x.IsStockQuantityAvailable(It.IsAny<int>())).Returns(true);
        var item = OrderItem.Create(1, 2, 500m, _stockMock.Object);
        var address = new Address("S", "C", "1", "C");

        // Act
        var order = Order.Create(Guid.NewGuid(), address, new[] { item });

        // Assert
        order.Status.Should().Be(OrderStatus.Pending);
        order.TotalAmount.Should().Be(1000m);
    }

    /// <summary>
    /// Подтверждение товара с валидными данными должно изменить статус на Confirmed
    /// </summary>
    [Fact]
    public void ConfirmOrder_WhenPendingAndHasPayment_ShouldChangeStatus()
    {
        // Arrange
        _stockMock.Setup(x => x.IsStockQuantityAvailable(It.IsAny<int>())).Returns(true);
        var item = OrderItem.Create(1, 1, 100m, _stockMock.Object);
        var order = Order.Create(Guid.NewGuid(), new Address("S", "C", "1", "C"), new[] { item });
        order.AssignPayment(123);

        // Act
        order.ConfirmOrder();

        // Assert
        order.Status.Should().Be(OrderStatus.Confirmed);
    }

    /// <summary>
    /// Поменять статус заказа на "Доставлен", когда к нему не привязан товар должен выбрасывать BusinessException
    /// </summary>
    [Fact]
    public void ShipOrder_WhenNoPayment_ShouldThrowBusinessException()
    {
        // Arrange
        _stockMock.Setup(x => x.IsStockQuantityAvailable(It.IsAny<int>())).Returns(true);
        var item = OrderItem.Create(1, 1, 100m, _stockMock.Object);
        var order = Order.Create(Guid.NewGuid(), new Address("S", "C", "1", "C"), new[] { item });

        // Act
        Action act = () => order.ShipOrder();

        // Assert
        act.Should().Throw<BusinessException>();
    }

    /// <summary>
    /// Добавление нового элемента заказа должно пересчитывать TotalAmount
    /// </summary>
    [Fact]
    public void UpdateOrderItem_AddingNewItem_ShouldRecalculateTotalAmount()
    {
        // Arrange
        _stockMock.Setup(x => x.IsStockQuantityAvailable(It.IsAny<int>())).Returns(true);
        var order = Order.Create(Guid.NewGuid(), new Address("S", "C", "1", "C"),
            new[] { OrderItem.Create(1, 1, 100m, _stockMock.Object) });

        // Act
        order.UpdateOrderItem(2, 1, 250m, _stockMock.Object);

        // Assert
        order.TotalAmount.Should().Be(350m);
        order.Items.Should().HaveCount(2);
    }
}
