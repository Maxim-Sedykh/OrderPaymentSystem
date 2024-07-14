﻿using Moq;
using OrderPaymentSystem.Application.Queries;
using OrderPaymentSystem.Domain.Dto.Product;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.UnitTests.Configurations;
using Xunit;

namespace OrderPaymentSystem.UnitTests.ProductTests
{
    public class GetProductTests : IClassFixture<ProductServiceFixture>
    {
        private readonly ProductServiceFixture _fixture = new ProductServiceFixture();

        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnProductDtoFromCache()
        {
            //Arrange
            var product = _fixture.GetProductDto();
            _fixture.CacheServiceMock.Setup(x => x.GetObjectAsync<ProductDto>(It.IsAny<string>()))
                .ReturnsAsync(product);

            // Act
            var result = await _fixture.ProductService.GetProductByIdAsync(product.Id);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(product.Id, result.Data.Id);
            Assert.Equal(product, result.Data);
            _fixture.CacheServiceMock.Verify(x => x.GetObjectAsync<ProductDto>(It.IsAny<string>()), Times.Once);
            _fixture.MediatorMock.Verify(x => x.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()), Times.Never);
            Assert.Equal("Test product #1", result.Data.ProductName);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldFetchProductFromDatabaseAndCache()
        {
            // Arrange
            var product = _fixture.GetProductDto();
            _fixture.CacheServiceMock.Setup(x => x.GetObjectAsync<ProductDto>(It.IsAny<string>()))
                .ReturnsAsync((ProductDto)null);
            _fixture.MediatorMock.Setup(x => x.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            // Act
            var result = await _fixture.ProductService.GetProductByIdAsync(1);

            // Assert
            Assert.Equal(product, result.Data);
            _fixture.CacheServiceMock.Verify(x => x.GetObjectAsync<ProductDto>(It.IsAny<string>()), Times.Once);
            _fixture.MediatorMock.Verify(x => x.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetProductByIdAsync_ProductNotFound_ReturnsErrorResult()
        {
            //Arrange
            _fixture.CacheServiceMock.Setup(x => x.GetObjectAsync<ProductDto>(It.IsAny<string>()))
                .ReturnsAsync((ProductDto)null);

            // Act
            var result = await _fixture.ProductService.GetProductByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(result.ErrorCode, (int)ErrorCodes.ProductNotFound);
        }
    }
}
