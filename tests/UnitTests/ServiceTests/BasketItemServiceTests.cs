using FluentAssertions;
using MapsterMapper;
using Moq;
using OrderPaymentSystem.Application.DTOs;
using OrderPaymentSystem.Application.DTOs.Basket;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Application.Services;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Domain.Resources;
using OrderPaymentSystem.Shared.Exceptions;
using OrderPaymentSystem.Shared.Specifications;
using Xunit;

namespace OrderPaymentSystem.UnitTests.ServiceTests
{
    public class BasketItemServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly BasketItemService _basketItemService;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IBasketItemRepository> _basketItemRepositoryMock;

        public BasketItemServiceTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();

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
            var product = Product.CreateExisting(productId, "Test Product", "Desc", 100m, 10);
            var basketItem = BasketItem.Create(userId, productId, quantity, product);
            var basketItemDto = new BasketItemDto { Id = 1, UserId = userId, ProductId = productId, Quantity = quantity };

            _productRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Product>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);
            _mapperMock.Setup(m => m.Map<BasketItemDto>(It.IsAny<BasketItem>())).Returns(basketItemDto);

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

            _productRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Product>>(),
                It.IsAny<CancellationToken>()))
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
            var product = Product.Create("Test Product", "Desc", 100m, 10);
            var basketItemId = 1L;
            var basketItem = BasketItem.CreateExisting(basketItemId, Guid.NewGuid(), 1, 2, product);

            _basketItemRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<BasketItem>>(),
                It.IsAny<CancellationToken>()))
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
            _basketItemRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<BasketItem>>(),
                It.IsAny<CancellationToken>()))
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
            var itemsDto = new List<BasketItemDto> 
            { 
                new() { Id = 1, Quantity = 2 },
                new() { Id = 2, Quantity = 1 } 
            };

            _basketItemRepositoryMock.Setup(r => r.GetListProjectedAsync<BasketItemDto>(It.IsAny<BaseSpecification<BasketItem>>(),
                It.IsAny<CancellationToken>()))
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

            var product = Product.CreateExisting(productId, "Test Product", "Desc", 100m, 10);

            var basketItem = BasketItem.CreateExisting(basketItemId, userId, productId, initialQuantity, product);
            basketItem.SetProduct(product);

            _basketItemRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(
                    It.IsAny<BaseSpecification<BasketItem>>(),
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
            basketItem.Quantity.Should().Be(newQuantity);

            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateQuantityAsync_ItemNotFound_ShouldReturnFailure()
        {
            // Arrange
            var basketItemId = 99L;
            var updateDto = new UpdateQuantityDto { NewQuantity = 10 };

            _basketItemRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<BasketItem>>(),
                It.IsAny<CancellationToken>()))
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
            var newQuantity = 0;
            var updateDto = new UpdateQuantityDto { NewQuantity = newQuantity };
            var userId = Guid.NewGuid();
            var productId = 1;
            var product = Product.CreateExisting(productId, "Test Product", "Desc", 100m, 10);
            var basketItem = BasketItem.CreateExisting(basketItemId, userId, productId, 5, product);
            basketItem.SetProduct(product);

            _basketItemRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<BasketItem>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(basketItem);

            // Act
            Func<Task> act = async () => await _basketItemService.UpdateQuantityAsync(basketItemId, updateDto);

            // Assert
            await act.Should().ThrowAsync<BusinessException>().WithMessage(ErrorMessage.QuantityPositive);
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }

}
