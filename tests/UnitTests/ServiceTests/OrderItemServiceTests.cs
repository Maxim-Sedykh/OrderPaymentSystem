using FluentAssertions;
using MapsterMapper;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;
using OrderPaymentSystem.Application.DTOs;
using OrderPaymentSystem.Application.DTOs.OrderItem;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Application.Services;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Domain.ValueObjects;
using OrderPaymentSystem.Shared.Specifications;
using Xunit;

namespace OrderPaymentSystem.UnitTests.ServiceTests
{
    public class OrderItemServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<OrderItemService>> _loggerMock;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IOrderItemRepository> _orderItemRepositoryMock;
        private readonly OrderItemService _orderItemService;

        // Секретный ключ для мока транзакции
        private readonly Mock<IDbContextTransaction> _transactionMock = new();

        public OrderItemServiceTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<OrderItemService>>();
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _productRepositoryMock = new Mock<IProductRepository>();
            _orderItemRepositoryMock = new Mock<IOrderItemRepository>();

            _uowMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            _uowMock.Setup(uow => uow.BeginTransactionAsync(It.IsAny<CancellationToken>())).ReturnsAsync(_transactionMock.Object);

            _orderItemService = new OrderItemService(_uowMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ValidData_ShouldCreateOrderItemAndReturnDto()
        {
            // Arrange
            var orderId = 1L;
            var productId = 1;
            var quantity = 3;
            var productPrice = 100m;
            var createDto = new CreateOrderItemDto { ProductId = productId, Quantity = quantity };

            var product = Product.CreateExisting(productId, "Test Product", "Desc", productPrice, 10);

            var order = Order.CreateExisting(orderId, Guid.NewGuid(), new Address("S", "C", "1", "Country"), new List<OrderItem>()
            {
                OrderItem.Create(product.Id, quantity, productPrice, product)
            }, 500m);

            var createdOrderItem = OrderItem.Create(productId, quantity, productPrice, product);
            var orderItemDto = new OrderItemDto { Id = 1, OrderId = orderId, ProductId = productId, Quantity = quantity, ItemTotalSum = quantity * productPrice };

            // Мокируем репозитории
            var orderRepositoryMock = new Mock<IOrderRepository>();
            orderRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Order>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);
            _uowMock.Setup(uow => uow.Orders).Returns(orderRepositoryMock.Object);

            var productRepositoryMock = new Mock<IProductRepository>();
            productRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Product>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);
            _uowMock.Setup(uow => uow.Products).Returns(productRepositoryMock.Object);

            // Мокируем маппер
            _mapperMock.Setup(m => m.Map<OrderItemDto>(It.IsAny<OrderItem>())).Returns(orderItemDto);

            // Act
            var result = await _orderItemService.CreateAsync(orderId, createDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.ProductId.Should().Be(productId);
            result.Data.Quantity.Should().Be(quantity);

            order.Items.Should().HaveCount(2); // Проверяем, чтоOrderItem был добавлен в заказ
            order.Items.First().ProductId.Should().Be(productId);

            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_OrderNotFound_ShouldReturnFailure()
        {
            // Arrange
            var orderId = 99L;
            var createDto = new CreateOrderItemDto { ProductId = 1, Quantity = 3 };

            _orderRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Order>>(), It.IsAny<CancellationToken>())).ReturnsAsync((Order)null);
            _productRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Product>>(), It.IsAny<CancellationToken>())).ReturnsAsync(Product.Create("P", "D", 100, 10));
            _uowMock.Setup(uow => uow.Orders).Returns(_orderRepositoryMock.Object);
            _uowMock.Setup(uow => uow.Products).Returns(_productRepositoryMock.Object);

            // Act
            var result = await _orderItemService.CreateAsync(orderId, createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.Order.NotFound(orderId));
        }

        [Fact]
        public async Task CreateAsync_ProductNotFound_ShouldReturnFailure()
        {
            // Arrange
            var orderId = 1L;
            var createDto = new CreateOrderItemDto { ProductId = 99, Quantity = 3 };
            var product = Product.CreateExisting(99, "test", "test", 500m, 3);

            var order = Order.CreateExisting(orderId, Guid.NewGuid(), new Address("S", "C", "1", "Country"), new List<OrderItem>()
            {
                OrderItem.Create(99, 3, 500m, product)
            }, 500m);

            _orderRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Order>>(), It.IsAny<CancellationToken>())).ReturnsAsync(order);
            _productRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Product>>(), It.IsAny<CancellationToken>())).ReturnsAsync((Product)null);
            _uowMock.Setup(uow => uow.Orders).Returns(_orderRepositoryMock.Object);
            _uowMock.Setup(uow => uow.Products).Returns(_productRepositoryMock.Object);

            // Act
            var result = await _orderItemService.CreateAsync(orderId, createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.Product.NotFound(createDto.ProductId));
        }

        [Fact]
        public async Task DeleteByIdAsync_ValidId_ShouldRemoveItemAndReturnSuccess()
        {
            // Arrange
            var orderItemId = 1L;
            var orderId = 1L;
            var product = Product.Create("P", "D", 100, 10);
            var orderItem = OrderItem.CreateExisting(orderItemId, 1, 2, product.Price, product);

            var order = Order.CreateExisting(orderId, Guid.NewGuid(), new Address("S", "C", "1", "Country"), new List<OrderItem> { orderItem }, 500m);

            // Мокируем репозитории
            _orderItemRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<OrderItem>>(), It.IsAny<CancellationToken>())).ReturnsAsync(orderItem);
            _orderRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Order>>(), It.IsAny<CancellationToken>())).ReturnsAsync(order); // Нужно, чтобы Order.RemoveOrderItem отработал
            _uowMock.Setup(uow => uow.OrderItems).Returns(_orderItemRepositoryMock.Object);
            _uowMock.Setup(uow => uow.Orders).Returns(_orderRepositoryMock.Object);

            // Act
            var result = await _orderItemService.DeleteByIdAsync(orderItemId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            order.Items.Should().BeEmpty(); // Проверяем, что элемент удален из заказа
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteByIdAsync_ItemNotFound_ShouldReturnFailure()
        {
            // Arrange
            _orderItemRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<OrderItem>>(), It.IsAny<CancellationToken>())).ReturnsAsync((OrderItem)null);
            _uowMock.Setup(uow => uow.OrderItems).Returns(_orderItemRepositoryMock.Object);

            // Act
            var result = await _orderItemService.DeleteByIdAsync(99L);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.Order.ItemNotFound(99L));
        }

        [Fact]
        public async Task GetByOrderIdAsync_ValidOrderId_ShouldReturnCollection()
        {
            // Arrange
            var orderId = 1L;
            var orderItemsDto = new List<OrderItemDto> { new OrderItemDto { Id = 1, Quantity = 2 }, new OrderItemDto { Id = 2, Quantity = 1 } };

            _orderItemRepositoryMock.Setup(r => r.GetListProjectedAsync<OrderItemDto>(It.IsAny<BaseSpecification<OrderItem>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(orderItemsDto);
            _uowMock.Setup(uow => uow.OrderItems).Returns(_orderItemRepositoryMock.Object);

            // Act
            var result = await _orderItemService.GetByOrderIdAsync(orderId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().HaveCount(2);
            result.Data.Should().BeEquivalentTo(orderItemsDto);
        }

        [Fact]
        public async Task UpdateQuantityAsync_ValidData_ShouldUpdateQuantityAndRecalculateTotal()
        {
            // Arrange
            var orderItemId = 1L;
            var newQuantity = 10;
            var updateDto = new UpdateQuantityDto { NewQuantity = newQuantity };
            var orderId = 1L;
            var productId = 1;
            var initialQuantity = 2;
            var productPrice = 100m;

            var product = Product.Create("P", "D", productPrice, 10);
            var orderItem = OrderItem.CreateExisting(orderItemId, productId, initialQuantity, productPrice, product);
            orderItem.SetProduct(product);


            var order = Order.CreateExisting(orderId, Guid.NewGuid(), new Address("S", "C", "1", "Country"), new List<OrderItem> { orderItem }, 500m);

            // Мокируем репозитории
            _orderItemRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(
                    It.IsAny<BaseSpecification<OrderItem>>(), // Проверяем, что WithProduct() был применен
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(orderItem);
            _orderRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Order>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(order); // Нужно для Order.UpdateOrderItemQuantity

            _uowMock.Setup(uow => uow.OrderItems).Returns(_orderItemRepositoryMock.Object);
            _uowMock.Setup(uow => uow.Orders).Returns(_orderRepositoryMock.Object);

            // Мокируем маппер для возврата DTO
            var updatedOrderItemDto = new OrderItemDto { Id = orderItemId, Quantity = newQuantity, ItemTotalSum = newQuantity * productPrice };
            _mapperMock.Setup(m => m.Map<OrderItemDto>(It.IsAny<OrderItem>())).Returns(updatedOrderItemDto);

            // Act
            var result = await _orderItemService.UpdateQuantityAsync(orderItemId, updateDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Quantity.Should().Be(newQuantity);
            result.Data.ItemTotalSum.Should().Be(newQuantity * productPrice);

            // Проверяем, что доменная сущность OrderItem обновилась
            var updatedOrderItemInOrder = order.Items.FirstOrDefault(oi => oi.Id == orderItemId);
            updatedOrderItemInOrder.Should().NotBeNull();
            updatedOrderItemInOrder.Quantity.Should().Be(newQuantity);
            updatedOrderItemInOrder.ItemTotalSum.Should().Be(newQuantity * productPrice);

            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateQuantityAsync_ItemNotFound_ShouldReturnFailure()
        {
            // Arrange
            _orderItemRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<OrderItem>>(), It.IsAny<CancellationToken>())).ReturnsAsync((OrderItem)null);
            _uowMock.Setup(uow => uow.OrderItems).Returns(_orderItemRepositoryMock.Object);

            // Act
            var result = await _orderItemService.UpdateQuantityAsync(99L, new UpdateQuantityDto { NewQuantity = 5 });

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.Order.ItemNotFound(99L));
        }
    }

}
