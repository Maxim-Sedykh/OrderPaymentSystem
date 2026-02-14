using FluentAssertions;
using Moq;
using OrderPaymentSystem.Application.DTOs.Order;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Domain.ValueObjects;
using OrderPaymentSystem.UnitTests.Configurations.Factories;
using OrderPaymentSystem.UnitTests.Configurations.Fixtures;
using Xunit;

namespace OrderPaymentSystem.UnitTests.ServiceTests;

public class OrderServiceTests
{
    private readonly OrderFixture _fixture;

    public OrderServiceTests()
    {
        _fixture = new OrderFixture();
    }

    [Fact]
    public async Task CompleteProcessingAsync_WhenValid_ShouldConfirmOrderAndReduceStock()
    {
        // Arrange
        var product = TestDataFactory.Product.WithStock(10).Build();
        var item = TestDataFactory.OrderItem.WithProduct(product).WithQuantity(3).Build();
        var order = TestDataFactory.Order.WithItems(item).WithStatus(OrderStatus.Pending).Build();
        var payment = TestDataFactory.Payment.WithStatus(PaymentStatus.Succeeded).Build();

        _fixture.SetupOrder(order)
                .SetupPayment(payment);

        // Act
        var result = await _fixture.Service.CompleteProcessingAsync(order.Id, payment.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Confirmed);
        product.StockQuantity.Should().Be(7);

        _fixture.VerifyTransactionCommitted();
        _fixture.VerifySaved();
    }

    [Fact]
    public async Task CreateAsync_WhenProductsExist_ShouldCreateOrder()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var p1 = TestDataFactory.Product.WithId(1).WithPrice(100).Build();
        var p2 = TestDataFactory.Product.WithId(2).WithPrice(200).Build();

        var dto = new CreateOrderDto
        {
            DeliveryAddress = new Address("S", "C", "Z", "Co"),
            OrderItems =
            [
                new() { ProductId = p1.Id, Quantity = 1 },
                new() { ProductId = p2.Id, Quantity = 1 }
            ]
        };

        _fixture.SetupProductsDictionary([p1, p2])
                .SetupMapping<Order, OrderDto>(new OrderDto());

        // Act
        var result = await _fixture.Service.CreateAsync(userId, dto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _fixture.OrderRepo.Verify(r => r.CreateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
        _fixture.VerifySaved();
    }

    [Fact]
    public async Task ShipOrderAsync_WhenOrderIsConfirmedAndPaid_ShouldSetStatusShipped()
    {
        // Arrange
        var payment = TestDataFactory.Payment.WithStatus(PaymentStatus.Succeeded).Build();
        var order = TestDataFactory.Order.WithStatus(OrderStatus.Confirmed).Build();
        order.AssignPayment(payment.Id);
        order.SetPayment(payment);

        _fixture.SetupOrder(order)
                .SetupPayment(payment);

        // Act
        var result = await _fixture.Service.ShipOrderAsync(order.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Shipped);
        _fixture.VerifySaved();
    }

    [Fact]
    public async Task ShipOrderAsync_WhenPaymentMissing_ShouldReturnError()
    {
        // Arrange
        var order = TestDataFactory.Order.WithStatus(OrderStatus.Pending).Build();
        _fixture.SetupOrder(order);

        // Act
        var result = await _fixture.Service.ShipOrderAsync(order.Id);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(DomainErrors.Order.CannotBeConfirmedWithoutPayment());
        _fixture.VerifyNotSaved();
    }
}
