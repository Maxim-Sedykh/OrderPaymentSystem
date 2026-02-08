using FluentAssertions;
using MapsterMapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using OrderPaymentSystem.Application.Constants;
using OrderPaymentSystem.Application.DTOs.Product;
using OrderPaymentSystem.Application.Interfaces.Cache;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Application.Services;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Shared.Specifications;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace OrderPaymentSystem.UnitTests.ServiceTests
{
    public class ProductServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<ILogger<ProductService>> _loggerMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ProductService _productService;

        // Моки репозиториев
        private readonly Mock<IProductRepository> _productRepositoryMock;

        public ProductServiceTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<ProductService>>();
            _cacheServiceMock = new Mock<ICacheService>();
            _mapperMock = new Mock<IMapper>();

            _productRepositoryMock = new Mock<IProductRepository>();

            _uowMock.Setup(uow => uow.Products).Returns(_productRepositoryMock.Object);
            _uowMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            _productService = new ProductService(
                _uowMock.Object,
                _loggerMock.Object,
                _cacheServiceMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task CreateAsync_NewProduct_ShouldCreateAndReturnSuccess()
        {
            // Arrange
            var createDto = new CreateProductDto("New Gadget", "A cool new gadget", 199.99m, 50);

            _productRepositoryMock.Setup(r => r.AnyAsync(It.IsAny<BaseSpecification<Product>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false); // Продукт не существует
            _productRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()));
            _cacheServiceMock.Setup(cs => cs.RemoveAsync(CacheKeys.Product.All, It.IsAny<CancellationToken>()));

            // Act
            var result = await _productService.CreateAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _productRepositoryMock.Verify(r => r.CreateAsync(It.Is<Product>(p =>
                p.Name == createDto.Name &&
                p.Price == createDto.Price &&
                p.StockQuantity == createDto.StockQuantity), It.IsAny<CancellationToken>()), Times.Once);
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _cacheServiceMock.Verify(cs => cs.RemoveAsync(CacheKeys.Product.All, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ProductAlreadyExists_ShouldReturnFailure()
        {
            // Arrange
            var createDto = new CreateProductDto("New Gadget", "A cool new gadget", 199.99m, 50);
            _productRepositoryMock.Setup(r => r.AnyAsync(It.IsAny<BaseSpecification<Product>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _productService.CreateAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.Product.AlreadyExist(createDto.Name));
            _productRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteByIdAsync_ValidId_ShouldDeleteProductAndInvalidateCache()
        {
            // Arrange
            var productId = 1;


            var product = Product.CreateExisting(productId, "Product To Delete", "Desc", 100m, 10);

            _productRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Product>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);
            _productRepositoryMock.Setup(r => r.Remove(It.IsAny<Product>()));
            _cacheServiceMock.Setup(cs => cs.RemoveAsync(CacheKeys.Product.All, It.IsAny<CancellationToken>()));
            _cacheServiceMock.Setup(cs => cs.RemoveAsync(CacheKeys.Product.ById(productId), It.IsAny<CancellationToken>()));

            // Act
            var result = await _productService.DeleteByIdAsync(productId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _productRepositoryMock.Verify(r => r.Remove(product), Times.Once);
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _cacheServiceMock.Verify(cs => cs.RemoveAsync(CacheKeys.Product.All, It.IsAny<CancellationToken>()), Times.Once);
            _cacheServiceMock.Verify(cs => cs.RemoveAsync(CacheKeys.Product.ById(productId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteByIdAsync_NotFound_ShouldReturnFailure()
        {
            // Arrange
            var productId = 99;
            _productRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Product>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product)null);

            // Act
            var result = await _productService.DeleteByIdAsync(productId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.Product.NotFound(productId));
            _productRepositoryMock.Verify(r => r.Remove(It.IsAny<Product>()), Times.Never);
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            _cacheServiceMock.Verify(cs => cs.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetByIdAsync_ProductFound_ShouldReturnFromCacheOrDbAndMapToDto()
        {
            // Arrange
            var productId = 1;
            var productDto = new ProductDto(productId, "Test", "Test", 5m, 5, DateTime.Now);

            // Имитируем кеш, который вернет DTO
            _cacheServiceMock.Setup(cs => cs.GetOrCreateAsync(CacheKeys.Product.ById(productId), It.IsAny<Func<CancellationToken, Task<ProductDto>>>()))
                .ReturnsAsync(productDto);

            // Act
            var result = await _productService.GetByIdAsync(productId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Id.Should().Be(productId);
            result.Data.Name.Should().Be("Test Product");

            _cacheServiceMock.Verify(cs => cs.GetOrCreateAsync(CacheKeys.Product.ById(productId), It.IsAny<Func<CancellationToken, Task<ProductDto>>>()));
            _productRepositoryMock.Verify(r => r.GetProjectedAsync<ProductDto>(It.IsAny<BaseSpecification<Product>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetByIdAsync_ProductNotFound_ShouldReturnFailure()
        {
            // Arrange
            var productId = 99;

            // Имитируем кеш, который не нашел и вызвал репозиторий, но репозиторий тоже ничего не вернул
            _cacheServiceMock.Setup(cs => cs.GetOrCreateAsync(CacheKeys.Product.ById(productId), It.IsAny<Func<CancellationToken, Task<ProductDto>>>()))
                .ReturnsAsync((ProductDto)null);

            // Act
            var result = await _productService.GetByIdAsync(productId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.Product.NotFound(productId));
        }

        [Fact]
        public async Task GetAllAsync_ProductsExist_ShouldReturnCollectionFromCache()
        {
            // Arrange
            var productsDto = new List<ProductDto> { new ProductDto(1, "Test", "Test", 5m, 5, DateTime.Now)
                , new ProductDto(2, "Test", "Test", 5m, 5, DateTime.Now) };

            _cacheServiceMock.Setup(cs => cs.GetOrCreateAsync(CacheKeys.Product.All, It.IsAny<Func<CancellationToken, Task<List<ProductDto>>>>()))
                .ReturnsAsync(productsDto);

            // Act
            var result = await _productService.GetAllAsync();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().HaveCount(2);
            _cacheServiceMock.Verify(cs => cs.GetOrCreateAsync(CacheKeys.Product.All, It.IsAny<Func<CancellationToken, Task<List<ProductDto>>>>()), Times.Once);
            _productRepositoryMock.Verify(r => r.GetListProjectedAsync<ProductDto>(It.IsAny<BaseSpecification<Product>>(), It.IsAny<CancellationToken>()), Times.Never); // Убедимся, что репозиторий не вызывался
        }

        [Fact]
        public async Task UpdateAsync_ValidData_ShouldUpdateProductAndInvalidateCache()
        {
            // Arrange
            var productId = 1;
            var updateDto = new UpdateProductDto
            {
                Name = "Updated Product",
                Description = "New Desc",
                Price = 150.50m,
                StockQuantity = 20
            };
            var product = Product.CreateExisting(productId, "Old Name", "Old Desc", 100m, 10);

            _productRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Product>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);
            _mapperMock.Setup(m => m.Map<ProductDto>(It.IsAny<Product>())).Returns(new ProductDto(1, "Test", "Test", 5m, 5, DateTime.Now)); // Пример маппинга
            _cacheServiceMock.Setup(cs => cs.RemoveAsync(CacheKeys.Product.All, It.IsAny<CancellationToken>()));
            _cacheServiceMock.Setup(cs => cs.RemoveAsync(CacheKeys.Product.ById(productId), It.IsAny<CancellationToken>()));

            // Act
            var result = await _productService.UpdateAsync(productId, updateDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            product.Name.Should().Be(updateDto.Name);
            product.Price.Should().Be(updateDto.Price);
            product.StockQuantity.Should().Be(updateDto.StockQuantity);

            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _cacheServiceMock.Verify(cs => cs.RemoveAsync(CacheKeys.Product.All, It.IsAny<CancellationToken>()), Times.Once);
            _cacheServiceMock.Verify(cs => cs.RemoveAsync(CacheKeys.Product.ById(productId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ProductNotFound_ShouldReturnFailure()
        {
            // Arrange
            var productId = 99;
            _productRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Product>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product)null);

            // Act
            var result = await _productService.UpdateAsync(productId, new UpdateProductDto());

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.Product.NotFound(productId));
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            _cacheServiceMock.Verify(cs => cs.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }

}
