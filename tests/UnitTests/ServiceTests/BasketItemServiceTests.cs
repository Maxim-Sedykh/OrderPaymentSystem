using FluentAssertions;
using MapsterMapper;
using Moq;
using OrderPaymentSystem.Application.DTOs;
using OrderPaymentSystem.Application.DTOs.Basket;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Application.Services;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace OrderPaymentSystem.UnitTests.ServiceTests
{
    public class BasketItemServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly BasketItemService _basketItemService;
        private readonly Mock<IProductRepository> _productRepositoryMock; // Мок репозитория продуктов
        private readonly Mock<IBasketItemRepository> _basketItemRepositoryMock; // Мок репозитория корзины

        public BasketItemServiceTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();

            // Мокируем репозитории, которые будут использоваться сервисом
            _productRepositoryMock = new Mock<IProductRepository>();
            _basketItemRepositoryMock = new Mock<IBasketItemRepository>();

            _uowMock.Setup(uow => uow.Products).Returns(_productRepositoryMock.Object);
            _uowMock.Setup(uow => uow.BasketItems).Returns(_basketItemRepositoryMock.Object);
            _uowMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            _basketItemService = new BasketItemService(_uowMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ValidData_ShouldCreateBasketItemAndReturnDto()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var productId = 1;
            var quantity = 5;
            var createDto = new CreateBasketItemDto { ProductId = productId, Quantity = quantity };
            var product = Product.Create("Test Product", "Desc", 100m, 10); // Stock 10, so quantity 5 is available
            var basketItem = BasketItem.Create(userId, productId, quantity, product);
            var basketItemDto = new BasketItemDto { Id = 1, UserId = userId, ProductId = productId, Quantity = quantity, Product = new BasketItemProductDto { Id = productId, Price = 100m } }; // Пример DTO

            // Мокируем репозитории
            _productRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Specification<Product>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);
            _basketItemRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<BasketItem>(), It.IsAny<CancellationToken>()))
                .Callback<BasketItem, CancellationToken>((bi, ct) => { /* Устанавливаем ID для теста */ bi.Id = 1; });
            _mapperMock.Setup(m => m.Map<BasketItemDto>(basketItem)).Returns(basketItemDto);

            // Act
            var result = await _basketItemService.CreateAsync(userId, createDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.ProductId.Should().Be(productId);
            result.Data.Quantity.Should().Be(quantity);

            _basketItemRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<BasketItem>(), It.IsAny<CancellationToken>()), Times.Once);
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ProductNotFound_ShouldReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var createDto = new CreateBasketItemDto { ProductId = 99, Quantity = 5 };

            // Мокируем репозиторий, чтобы он не нашел продукт
            _productRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Specification<Product>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product)null);

            // Act
            var result = await _basketItemService.CreateAsync(userId, createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.Product.NotFound(createDto.ProductId));
            _basketItemRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<BasketItem>(), It.IsAny<CancellationToken>()), Times.Never);
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteByIdAsync_ValidId_ShouldDeleteAndReturnSuccess()
        {
            // Arrange
            var basketItemId = 1L;
            var basketItem = new BasketItem { Id = basketItemId, UserId = Guid.NewGuid(), ProductId = 1, Quantity = 2 };

            // Мокируем репозитории
            _basketItemRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Specification<BasketItem>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(basketItem);
            _basketItemRepositoryMock.Setup(r => r.Remove(It.IsAny<BasketItem>()));

            // Act
            var result = await _basketItemService.DeleteByIdAsync(basketItemId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _basketItemRepositoryMock.Verify(r => r.Remove(basketItem), Times.Once);
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteByIdAsync_NotFound_ShouldReturnFailure()
        {
            // Arrange
            var basketItemId = 99L;
            _basketItemRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Specification<BasketItem>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((BasketItem)null);

            // Act
            var result = await _basketItemService.DeleteByIdAsync(basketItemId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.BasketItem.NotFound(basketItemId));
            _basketItemRepositoryMock.Verify(r => r.Remove(It.IsAny<BasketItem>()), Times.Never);
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetByUserIdAsync_ValidUserId_ShouldReturnCollectionOfItems()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var itemsDto = new List<BasketItemDto> { new BasketItemDto { Id = 1, Quantity = 2 }, new BasketItemDto { Id = 2, Quantity = 1 } };

            _basketItemRepositoryMock.Setup(r => r.GetListProjectedAsync<BasketItemDto>(It.IsAny<Specification<BasketItem>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(itemsDto);

            // Act
            var result = await _basketItemService.GetByUserIdAsync(userId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().HaveCount(2);
            result.Data.Should().BeEquivalentTo(itemsDto);
        }

        [Fact]
        public async Task UpdateQuantityAsync_ValidData_ShouldUpdateAndReturnDto()
        {
            // Arrange
            var basketItemId = 1L;
            var newQuantity = 10;
            var updateDto = new UpdateQuantityDto { NewQuantity = newQuantity };
            var userId = Guid.NewGuid();
            var productId = 1;
            var initialQuantity = 5;

            // Создаем продукт, который будет использоваться как StockInfo
            var product = Product.Create("Test Product", "Desc", 100m, 10); // Stock 10

            // Имитируем BasketItem с product, который был загружен через WithProduct()
            var basketItem = BasketItem.Create(userId, productId, initialQuantity, product);
            basketItem.Id = basketItemId; // Устанавливаем ID для теста

            // Настройка моков
            _basketItemRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(
                    It.Is<Specification<BasketItem>>(s => s.Predicate(basketItem) && s.Includes.Any(i => i.PropertyName == "Product")), // Проверяем, что WithProduct() был применен
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(basketItem);

            var updatedBasketItemDto = new BasketItemDto { Id = basketItemId, Quantity = newQuantity };
            _mapperMock.Setup(m => m.Map<BasketItemDto>(basketItem)).Returns(updatedBasketItemDto);

            // Act
            var result = await _basketItemService.UpdateQuantityAsync(basketItemId, updateDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Quantity.Should().Be(newQuantity);
            basketItem.Quantity.Should().Be(newQuantity); // Проверяем, что доменная сущность обновилась

            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateQuantityAsync_ItemNotFound_ShouldReturnFailure()
        {
            // Arrange
            var basketItemId = 99L;
            var updateDto = new UpdateQuantityDto { NewQuantity = 10 };

            _basketItemRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Specification<BasketItem>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((BasketItem)null);

            // Act
            var result = await _basketItemService.UpdateQuantityAsync(basketItemId, updateDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.BasketItem.NotFound(basketItemId));
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdateQuantityAsync_InvalidQuantityUpdate_ShouldThrowBusinessException()
        {
            // Arrange
            var basketItemId = 1L;
            var newQuantity = 0; // Невалидное количество
            var updateDto = new UpdateQuantityDto { NewQuantity = newQuantity };
            var userId = Guid.NewGuid();
            var productId = 1;
            var product = Product.Create("Test Product", "Desc", 100m, 10);
            var basketItem = BasketItem.Create(userId, productId, 5, product);
            basketItem.Id = basketItemId;

            _basketItemRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Specification<BasketItem>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(basketItem);

            // Act
            Func<Task> act = async () => await _basketItemService.UpdateQuantityAsync(basketItemId, updateDto);

            // Assert
            await act.Should().ThrowAsync<BusinessException>().WithMessage("Количество товара должно быть положительным."); // Проверяем сообщение исключения
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }

}
