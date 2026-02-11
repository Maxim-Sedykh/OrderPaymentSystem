using FluentAssertions;
using MapsterMapper;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;
using OrderPaymentSystem.Application.DTOs.Order;
using OrderPaymentSystem.Application.DTOs.OrderItem;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Application.Services;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Domain.ValueObjects;
using OrderPaymentSystem.Shared.Specifications;
using Xunit;

namespace OrderPaymentSystem.UnitTests.ServiceTests
{
    public class OrderServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<OrderService>> _loggerMock;
        private readonly OrderService _orderService;
        private readonly Mock<IDbContextTransaction> _transactionMock = new();

        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IOrderItemRepository> _orderItemRepositoryMock;

        public OrderServiceTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<OrderService>>();

            _uowMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            _uowMock.Setup(uow => uow.BeginTransactionAsync(It.IsAny<CancellationToken>())).ReturnsAsync(_transactionMock.Object);

            _orderRepositoryMock = new Mock<IOrderRepository>();
            _paymentRepositoryMock = new Mock<IPaymentRepository>();
            _productRepositoryMock = new Mock<IProductRepository>();
            _orderItemRepositoryMock = new Mock<IOrderItemRepository>();

            _uowMock.Setup(uow => uow.Orders).Returns(_orderRepositoryMock.Object);
            _uowMock.Setup(uow => uow.Payments).Returns(_paymentRepositoryMock.Object);
            _uowMock.Setup(uow => uow.Products).Returns(_productRepositoryMock.Object);
            _uowMock.Setup(uow => uow.OrderItems).Returns(_orderItemRepositoryMock.Object);

            _orderService = new OrderService(_loggerMock.Object, _mapperMock.Object, _uowMock.Object);
        }

        [Fact]
        public async Task CompleteProcessingAsync_ValidData_ShouldReduceStockAndConfirmOrder()
        {
            // Arrange
            var orderId = 1L;
            var paymentId = 101L;
            var userId = Guid.NewGuid();
            var product = Product.CreateExisting(1, "Laptop", "Desc", 1000m, 10);
            var orderItem = OrderItem.Create(product.Id, 2, product.Price, product);
            orderItem.SetProduct(product);
            var order = Order.CreateExisting(orderId, userId, new Address("S", "C", "1", "Country"), new List<OrderItem> { orderItem }, 1000m, OrderStatus.Pending);// Назначаем платеж

            var payment = Payment.CreateExisting(paymentId, orderId, 2000m, 2000m, PaymentMethod.Cash, PaymentStatus.Succeeded);

            _orderRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Order>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);
            _paymentRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Payment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(payment);
            _uowMock.Setup(uow => uow.Orders).Returns(_orderRepositoryMock.Object);
            _uowMock.Setup(uow => uow.Payments).Returns(_paymentRepositoryMock.Object);

            // Act
            var result = await _orderService.CompleteProcessingAsync(orderId, paymentId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            order.Status.Should().Be(OrderStatus.Confirmed);
            product.StockQuantity.Should().Be(8);
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CompleteProcessingAsync_OrderNotFound_ShouldReturnFailure()
        {
            // Arrange
            _orderRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Order>>(), It.IsAny<CancellationToken>())).ReturnsAsync((Order)null);
            _uowMock.Setup(uow => uow.Orders).Returns(_orderRepositoryMock.Object);

            // Act
            var result = await _orderService.CompleteProcessingAsync(99L, 101L);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.Order.NotFound(99L));
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CompleteProcessingAsync_PaymentNotFound_ShouldReturnFailure()
        {
            var orderItem = OrderItem.CreateExisting(1, 1, 1, 500m, Product.Create("test", "test", 500m, 50));

            // Arrange
            var orderId = 1L;
            var paymentId = 101L;
            var order = Order.CreateExisting(orderId, Guid.NewGuid(), new Address("S", "C", "1", "Country"), new List<OrderItem>()
            {
                orderItem
            }, 100m, OrderStatus.Pending);

            _orderRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Order>>(), It.IsAny<CancellationToken>())).ReturnsAsync(order);
            _paymentRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Payment>>(), It.IsAny<CancellationToken>())).ReturnsAsync((Payment)null);
            _uowMock.Setup(uow => uow.Orders).Returns(_orderRepositoryMock.Object);
            _uowMock.Setup(uow => uow.Payments).Returns(_paymentRepositoryMock.Object);

            // Act
            var result = await _orderService.CompleteProcessingAsync(orderId, paymentId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.Payment.NotFound(paymentId));
        }

        [Fact]
        public async Task CreateAsync_ValidOrder_ShouldCreateAndReturnDto()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var address = new Address("S", "C", "1", "Country");
            var createDto = new CreateOrderDto { DeliveryAddress = address, OrderItems = new List<CreateOrderItemDto>() };

            var product1 = Product.CreateExisting(1, "Product A", "Desc A", 100m, 10);
            var product2 = Product.CreateExisting(2, "Product B", "Desc B", 200m, 5);

            createDto.OrderItems.Add(new CreateOrderItemDto { ProductId = product1.Id, Quantity = 2 });
            createDto.OrderItems.Add(new CreateOrderItemDto { ProductId = product2.Id, Quantity = 1 });

            var productsDict = new Dictionary<int, Product>
            {
                { product1.Id, product1 },
                { product2.Id, product2 }
            };
            _productRepositoryMock.Setup(r => r.GetProductsAsDictionaryByIdAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(productsDict);
            _orderRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()));

            _uowMock.Setup(uow => uow.Products).Returns(_productRepositoryMock.Object);
            _uowMock.Setup(uow => uow.Orders).Returns(_orderRepositoryMock.Object);
            _mapperMock.Setup(m => m.Map<OrderDto>(It.IsAny<Order>())).Returns(new OrderDto());

            // Act
            var result = await _orderService.CreateAsync(userId, createDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateAsync_ProductNotFound_ShouldReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var address = new Address("S", "C", "1", "Country");
            var createDto = new CreateOrderDto { DeliveryAddress = address, OrderItems = new List<CreateOrderItemDto>() };
            createDto.OrderItems.Add(new CreateOrderItemDto { ProductId = 99, Quantity = 2 });

            _productRepositoryMock.Setup(r => r.GetProductsAsDictionaryByIdAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Dictionary<int, Product>());

            _uowMock.Setup(uow => uow.Products).Returns(_productRepositoryMock.Object);

            // Act
            var result = await _orderService.CreateAsync(userId, createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.Product.NotFound(99));
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ShipOrderAsync_ValidOrder_ShouldShipOrder()
        {
            // Arrange
            var orderId = 1L;
            var userId = Guid.NewGuid();
            var product = Product.CreateExisting(1, "P", "D", 100m, 10);
            var orderItem = OrderItem.Create(product.Id, 2, product.Price, product);
            orderItem.SetProduct(product);
            var payment = Payment.CreateExisting(101L, orderId, 2000, 1000, PaymentMethod.GiftCard, PaymentStatus.Succeeded);
            var order = Order.CreateExisting(orderId, userId, new Address("S", "C", "1", "Country"), new List<OrderItem> { orderItem }, 1000m, OrderStatus.Pending);
            order.SetPayment(payment);
            order.AssignPayment(payment.Id);
            order.ConfirmOrder();

            _orderRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Order>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);
            _uowMock.Setup(uow => uow.Orders).Returns(_orderRepositoryMock.Object);

            // Act
            var result = await _orderService.ShipOrderAsync(orderId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            order.Status.Should().Be(OrderStatus.Shipped);
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ShipOrderAsync_OrderNotFound_ShouldReturnFailure()
        {
            // Arrange
            _orderRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Order>>(), It.IsAny<CancellationToken>())).ReturnsAsync((Order)null);
            _uowMock.Setup(uow => uow.Orders).Returns(_orderRepositoryMock.Object);

            // Act
            var result = await _orderService.ShipOrderAsync(99L);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.Order.NotFound(99L));
        }

        [Fact]
        public async Task ShipOrderAsync_OrderWithoutPayment_ShouldReturnFailure()
        {
            // Arrange
            var orderItem = OrderItem.CreateExisting(1, 1, 1, 500m, Product.Create("test", "test", 500m, 50));

            var orderId = 1L;
            var order = Order.CreateExisting(orderId, Guid.NewGuid(), new Address("S", "C", "1", "Country"), new List<OrderItem>()
            {
                orderItem
            }, 1000m, OrderStatus.Pending);

            _orderRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Order>>(), It.IsAny<CancellationToken>())).ReturnsAsync(order);
            _uowMock.Setup(uow => uow.Orders).Returns(_orderRepositoryMock.Object);

            // Act
            var result = await _orderService.ShipOrderAsync(orderId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.Order.CannotBeConfirmedWithoutPayment());
        }

        [Fact]
        public async Task ShipOrderAsync_PaymentNotSucceeded_ShouldReturnFailure()
        {
            // Arrange
            var orderId = 1L;
            var paymentId = 101L;
            var product = Product.CreateExisting(1, "P", "D", 100m, 10);
            var orderItem = OrderItem.Create(product.Id, 2, product.Price, product);
            var order = Order.CreateExisting(orderId, Guid.NewGuid(), new Address("S", "C", "1", "Country"), new List<OrderItem> { orderItem }, 1000m, OrderStatus.Pending);
            order.AssignPayment(paymentId);

            var payment = Payment.CreateExisting(paymentId, orderId, 200m, 200m, PaymentMethod.Cash, PaymentStatus.Pending);

            _orderRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Order>>(), It.IsAny<CancellationToken>())).ReturnsAsync(order);
            _paymentRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Payment>>(), It.IsAny<CancellationToken>())).ReturnsAsync(payment);
            _uowMock.Setup(uow => uow.Orders).Returns(_orderRepositoryMock.Object);
            _uowMock.Setup(uow => uow.Payments).Returns(_paymentRepositoryMock.Object);

            // Act
            var result = await _orderService.ShipOrderAsync(orderId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.Order.CannotBeConfirmedWithoutPayment());
        }
    }
}
