using AutoMapper;
using Microsoft.Extensions.Options;
using Moq;
using OrderPaymentSystem.Application.Services;
using OrderPaymentSystem.Domain.Dto.Product;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces.Cache;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Settings;
using OrderPaymentSystem.Producer.Interfaces;
using OrderPaymentSystem.UnitTests.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OrderPaymentSystem.UnitTests.ProductTests
{
    public class GetProductTests : IClassFixture<ProductServiceFixture>
    {
        private readonly ProductService _productService;
        private readonly Mock<ICacheService> _redisCacheService;

        public GetProductTests(ProductServiceFixture fixture)
        {
            _productService = fixture.ProductService;
            _redisCacheService = fixture.CacheServiceMock;
        }

        [Fact]
        public async Task GetProductByIdAsync_ProductFound_ReturnsProductDto()
        {
            // Arrange
            var productId = 1;
            var cachedProductDto = new ProductDto { Id = productId, ProductName = "TestProduct", Description = "Test description", Cost = 500 };

            _redisCacheService.Setup(mock => mock.GetObjectAsync(It.IsAny<string>(), It.IsAny<Func<Task<ProductDto>>>()))
                .ReturnsAsync(cachedProductDto);

            // Act
            var result = await _productService.GetProductByIdAsync(productId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(productId, result.Data.Id);
            Assert.Equal("TestProduct", result.Data.ProductName);
        }

        [Fact]
        public async Task GetProductByIdAsync_ProductNotFound_ReturnsErrorResult()
        {
            _redisCacheService.Setup(mock => mock.GetObjectAsync(It.IsAny<string>(), It.IsAny<Func<Task<ProductDto>>>()))
                .ReturnsAsync((ProductDto)null);

            // Act
            var result = await _productService.GetProductByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(result.ErrorCode, (int)ErrorCodes.ProductNotFound);
        }
    }
}
