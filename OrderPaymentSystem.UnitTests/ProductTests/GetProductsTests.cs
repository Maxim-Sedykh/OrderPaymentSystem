using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using OrderPaymentSystem.Application.Queries;
using OrderPaymentSystem.Application.Services;
using OrderPaymentSystem.Domain.Dto.Product;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces.Cache;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Settings;
using OrderPaymentSystem.Producer.Interfaces;
using OrderPaymentSystem.UnitTests.Configurations;
using Serilog;
using Xunit;

namespace OrderPaymentSystem.UnitTests.ProductTests
{
    public class GetProductsTests : IClassFixture<ProductServiceFixture>
    {
        private readonly ProductServiceFixture _fixture;

        public GetProductsTests()
        {
            _fixture = new ProductServiceFixture();
        }

        [Fact]
        public async Task GetProductsAsync_ProductsFound_ShouldReturnProductDtosFromCache()
        {
            var products = _fixture.GetProductDtos();

            _fixture.CacheServiceMock.Setup(x => x.GetObjectAsync<ProductDto[]>(It.IsAny<string>()))
                .ReturnsAsync(products);

            // Act
            var result = await _fixture.ProductService.GetProductsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task GetProductsAsync_ShouldFetchProductsFromDatabaseAndCache()
        {
            // Arrange
            var products = _fixture.GetProductDtos();

            _fixture.CacheServiceMock.Setup(x => x.GetObjectAsync<ProductDto[]>(It.IsAny<string>()))
                .ReturnsAsync((ProductDto[])null);
            _fixture.MediatorMock.Setup(x => x.Send(It.IsAny<GetProductsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            // Act
            var result = await _fixture.ProductService.GetProductsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.NotEqual(0, result.Count);
            Assert.IsAssignableFrom<IEnumerable<ProductDto>>(result.Data);
        }

        [Fact]
        public async Task GetProducts_ProductsNotFound_ReturnsErrorResult()
        {
            //Arrange
            _fixture.CacheServiceMock.Setup(x => x.GetObjectAsync<ProductDto[]>(It.IsAny<string>()))
                .ReturnsAsync((ProductDto[])null);

            // Act
            var result = await _fixture.ProductService.GetProductsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(result.ErrorCode, (int)ErrorCodes.ProductsNotFound);
        }
    }
}
