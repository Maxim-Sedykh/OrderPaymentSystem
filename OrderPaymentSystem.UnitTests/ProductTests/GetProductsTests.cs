using AutoMapper;
using Moq;
using OrderPaymentSystem.Application.Services;
using OrderPaymentSystem.Domain.Dto.Product;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces.Cache;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using OrderPaymentSystem.UnitTests.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OrderPaymentSystem.UnitTests.ProductTests
{
    public class GetProductsTests : IClassFixture<ProductServiceFixture>
    {
        private readonly ProductService _productService;
        private readonly Mock<ICacheService> _redisCacheService;

        public GetProductsTests(ProductServiceFixture fixture)
        {
            _productService = fixture.ProductService;
            _redisCacheService = fixture.CacheServiceMock;
        }

        [Fact]
        public async Task GetProductsAsync_ProductsFound_ReturnsSuccessResultProductDtos()
        {
            // Arrange
            var cachedProductDto = new ProductDto { Id = 1, ProductName = "TestProduct", Description = "Test description", Cost = 500 };

            _redisCacheService.Setup(mock => mock.GetObjectAsync(It.IsAny<string>(), It.IsAny<Func<Task<List<ProductDto>>>>()))
                .ReturnsAsync(new List<ProductDto> { cachedProductDto });

            // Act
            var result = await _productService.GetProductsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.NotEqual(0, result.Count);
            Assert.IsAssignableFrom<IEnumerable<ProductDto>>(result.Data);
        }

        [Fact]
        public async Task GetProductByIdAsync_ProductNotFound_ReturnsErrorResult()
        {
            //Arrange
            _redisCacheService.Setup(mock => mock.GetObjectAsync(It.IsAny<string>(), It.IsAny<Func<Task<List<ProductDto>>>>()))
                .ReturnsAsync(new List<ProductDto>());

            // Act
            var result = await _productService.GetProductsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(result.ErrorCode, (int)ErrorCodes.ProductsNotFound);
        }
    }
}
