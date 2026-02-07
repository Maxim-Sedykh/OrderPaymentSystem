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
using System;
using System.Collections.Generic;
using System.Text;
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

        // Моки репозиториев, чтобы имитировать взаимодействие с БД
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IOrderItemRepository> _orderItemRepositoryMock;

        public OrderServiceTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<OrderService>>();

            // Базовые настройки
            _uowMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            _uowMock.Setup(uow => uow.BeginTransactionAsync(It.IsAny<CancellationToken>())).ReturnsAsync(_transactionMock.Object);

            // Мокируем репозитории, используемые в сервисе
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
            var product = Product.Create("Laptop", "Desc", 1000m, 10);
            var orderItem = OrderItem.Create(product.Id, 2, product.Price, product);
            var order = Order.Create(userId, new Address("S", "C", "1", "Country"), new List<OrderItem> { orderItem });
            order.Id = orderId;
            order.AssignPayment(paymentId); // Назначаем платеж

            var payment = Payment.Create(orderId, 2000m, 2000m, PaymentMethod.Card);
            payment.Id = paymentId;
            payment.Status = PaymentStatus.Succeeded;

            // Мокируем репозитории
            _orderRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.Is<Specification<Order>>(s => s.Predicate(order) && s.Includes.Any(i => i.PropertyName == "Items") && s.Includes.Any(i => i.PropertyName == "Product")), It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);
            _paymentRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.Is<Specification<Payment>>(s => s.Predicate(payment)), It.IsAny<CancellationToken>()))
                .ReturnsAsync(payment);
            _uowMock.Setup(uow => uow.Orders).Returns(_orderRepositoryMock.Object);
            _uowMock.Setup(uow => uow.Payments).Returns(_paymentRepositoryMock.Object);

            // Act
            var result = await _orderService.CompleteProcessingAsync(orderId, paymentId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            order.Status.Should().Be(OrderStatus.Confirmed); // Проверяем, что статус заказа изменился
            product.StockQuantity.Should().Be(8); // Проверяем, что количество товара уменьшилось
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CompleteProcessingAsync_OrderNotFound_ShouldReturnFailure()
        {
            // Arrange
            _orderRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Specification<Order>>(), It.IsAny<CancellationToken>())).ReturnsAsync((Order)null);
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
            // Arrange
            var orderId = 1L;
            var paymentId = 101L;
            var order = Order.Create(Guid.NewGuid(), new Address("S", "C", "1", "Country"), new List<OrderItem>());
            order.Id = orderId;

            _orderRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Specification<Order>>(), It.IsAny<CancellationToken>())).ReturnsAsync(order);
            _paymentRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Specification<Payment>>(), It.IsAny<CancellationToken>())).ReturnsAsync((Payment)null);
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

            var product1 = Product.Create("Product A", "Desc A", 100m, 10);
            product1.Id = 1;
            var product2 = Product.Create("Product B", "Desc B", 200m, 5);
            product2.Id = 2;

            createDto.OrderItems.Add(new CreateOrderItemDto { ProductId = product1.Id, Quantity = 2 });
            createDto.OrderItems.Add(new CreateOrderItemDto { ProductId = product2.Id, Quantity = 1 });

            // Мокируем репозитории
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

            // Mock mapper
            _mapperMock.Setup(m => m.Map<OrderDto>(It.IsAny<Order>())).Returns(new OrderDto());

            // Act
            var result = await _orderService.CreateAsync(userId, createDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();

            // Проверяем, что созданный заказ содержит правильные items
            // (Нам нужен объект Order, который был создан, чтобы проверить его)
            Order createdOrder = null;
            _orderRepositoryMock.Setup(r => r.CreateAsync(Capture.Do<Order>(order => createdOrder = order), It.IsAny<CancellationToken>()));
            await _orderService.CreateAsync(userId, createDto); // Повторный вызов для захвата объекта

            createdOrder.Should().NotBeNull();
            createdOrder.Items.Should().HaveCount(2);
            createdOrder.Items.Sum(i => i.Quantity * i.ProductPrice).Should().Be(400m); // 2*100 + 1*200
            createdOrder.UserId.Should().Be(userId);
            createdOrder.DeliveryAddress.Should().BeEquivalentTo(address);
            createdOrder.Status.Should().Be(OrderStatus.Pending);

            _uowMock.Verify(uow => uow.Orders.CreateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ProductNotFound_ShouldReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var address = new Address("S", "C", "1", "Country");
            var createDto = new CreateOrderDto { DeliveryAddress = address, OrderItems = new List<CreateOrderItemDto>() };
            createDto.OrderItems.Add(new CreateOrderItemDto { ProductId = 99, Quantity = 2 }); // Несуществующий продукт

            _productRepositoryMock.Setup(r => r.GetProductsAsDictionaryByIdAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Dictionary<int, Product>()); // Пустой словарь, продукт не найден

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
            var product = Product.Create("P", "D", 100m, 10);
            var orderItem = OrderItem.Create(product.Id, 2, product.Price, product);
            var order = Order.Create(userId, new Address("S", "C", "1", "Country"), new List<OrderItem> { orderItem });
            order.Id = orderId;
            order.ConfirmOrder(); // Начальный статус - Confirmed
            order.AssignPayment(101L); // Назначаем платеж

            // Мокируем репозиторий
            _orderRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.Is<Specification<Order>>(s => s.Predicate(order) && s.Includes.Any(i => i.PropertyName == "Items") && s.Includes.Any(i => i.PropertyName == "Payment") && s.Includes.Any(i => i.PropertyName == "Product")), It.IsAny<CancellationToken>()))
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
            _orderRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Specification<Order>>(), It.IsAny<CancellationToken>())).ReturnsAsync((Order)null);
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
            var orderId = 1L;
            var order = Order.Create(Guid.NewGuid(), new Address("S", "C", "1", "Country"), new List<OrderItem>());
            order.Id = orderId;

            _orderRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Specification<Order>>(), It.IsAny<CancellationToken>())).ReturnsAsync(order);
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
            var product = Product.Create("P", "D", 100m, 10);
            var orderItem = OrderItem.Create(product.Id, 2, product.Price, product);
            var order = Order.Create(Guid.NewGuid(), new Address("S", "C", "1", "Country"), new List<OrderItem> { orderItem });
            order.Id = orderId;
            order.AssignPayment(paymentId); // Назначаем платеж

            var payment = Payment.Create(orderId, 200m, 200m, PaymentMethod.Card);
            payment.Id = paymentId;
            payment.Status = PaymentStatus.Pending; // Платеж не оплачен

            _orderRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Specification<Order>>(), It.IsAny<CancellationToken>())).ReturnsAsync(order);
            _paymentRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Specification<Payment>>(), It.IsAny<CancellationToken>())).ReturnsAsync(payment);
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
