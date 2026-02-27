using FluentAssertions;
using OrderPaymentSystem.Application.DTOs;
using OrderPaymentSystem.Application.DTOs.OrderItem;
using OrderPaymentSystem.Application.Services.Orders;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.UnitTests.Configurations.Factories;
using OrderPaymentSystem.UnitTests.Configurations.Fixtures;

namespace OrderPaymentSystem.UnitTests.ServiceTests;

/// <summary>
/// Тесты сервиса <see cref="OrderItemService"/>
/// </summary>
public class OrderItemServiceTests
{
    private readonly OrderItemFixture _fixture;

    /// <summary>
    /// Конструктор. Инициализация фикстуры
    /// </summary>
    public OrderItemServiceTests()
    {
        _fixture = new OrderItemFixture();
    }

    /// <summary>
    /// При создании элемента заказа с валидными данными - 
    /// Он должен добавиться в заказ и должна пересчитаться TotalAmount у заказа
    /// </summary>
    [Fact]
    public async Task CreateAsync_WhenValid_ShouldAddItemToOrderAndRecalculateTotal()
    {
        // Arrange
        var product = TestDataFactory.Product.WithPrice(200m).Build();
        var order = TestDataFactory.Order.WithItems(TestDataFactory.OrderItem.Build()).Build();
        var dto = new CreateOrderItemDto { ProductId = product.Id, Quantity = 2 };

        _fixture.SetupOrder(order)
                .SetupProduct(product)
                .SetupMapping<OrderItem, OrderItemDto>(new OrderItemDto { ProductId = product.Id, Quantity = 2 });

        // Act
        var result = await _fixture.Service.CreateAsync(order.Id, dto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        order.Items.Should().HaveCount(2);
        order.TotalAmount.Should().Be(500m);
        _fixture.VerifySaved();
    }

    /// <summary>
    /// Удаление элемента из заказа где этот элемент существует
    /// Должно быть успешным, и так же уменьшать TotalAmount у заказа
    /// </summary>
    [Fact]
    public async Task DeleteByIdAsync_WhenItemExists_ShouldRemoveFromOrderAndDecrementTotal()
    {
        // Arrange
        var product = TestDataFactory.Product.WithPrice(100m).Build();
        var item = TestDataFactory.OrderItem.WithProduct(product).WithQuantity(3).Build();
        var order = TestDataFactory.Order.WithItems(item).Build();

        _fixture.SetupOrderItem(item)
                .SetupOrder(order);

        // Act
        var result = await _fixture.Service.DeleteByIdAsync(item.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        order.Items.Should().BeEmpty();
        order.TotalAmount.Should().Be(0);
        _fixture.VerifySaved();
    }

    /// <summary>
    /// Обновление количества товара в элементе должно пересчитать TotalAmount у заказа
    /// </summary>
    [Fact]
    public async Task UpdateQuantityAsync_WhenValid_ShouldUpdateItemAndOrderTotal()
    {
        // Arrange
        var product = TestDataFactory.Product.WithPrice(100m).WithStock(10).Build();
        var item = TestDataFactory.OrderItem.WithProduct(product).WithQuantity(1).Build();
        var order = TestDataFactory.Order.WithItems(item).Build();

        const int newQty = 5;
        _fixture.SetupOrderItem(item)
                .SetupOrder(order)
                .SetupMapping<OrderItem, OrderItemDto>(new OrderItemDto { Quantity = newQty });

        // Act
        var result = await _fixture.Service.UpdateQuantityAsync(item.Id, new UpdateQuantityDto { NewQuantity = newQty });

        // Assert
        result.IsSuccess.Should().BeTrue();
        item.Quantity.Should().Be(newQty);
        order.TotalAmount.Should().Be(500m);
        _fixture.VerifySaved();
    }

    /// <summary>
    /// Создание элемента для заказа которого не существует - должно завершиться с ошибкой
    /// </summary>
    [Fact]
    public async Task CreateAsync_WhenOrderNotFound_ShouldReturnError()
    {
        // Arrange
        _fixture.SetupOrder(null);

        // Act
        var result = await _fixture.Service.CreateAsync(999, new CreateOrderItemDto());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(DomainErrors.Order.NotFound(999));
        _fixture.VerifyNotSaved();
    }
}
