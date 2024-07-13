using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Moq;
using OrderPaymentSystem.Application.Queries;
using OrderPaymentSystem.Application.Services;
using OrderPaymentSystem.Domain.Dto.Product;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces.Cache;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Settings;
using OrderPaymentSystem.Producer.Interfaces;
using OrderPaymentSystem.UnitTests.Configurations;
using Serilog;
using Xunit;

namespace OrderPaymentSystem.UnitTests.ProductTests
{
    public class GetProductTests : IClassFixture<ProductServiceFixture>
    {
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IMessageProducer> _messageProducerMock;
        private readonly Mock<IOptions<RabbitMqSettings>> _rabbitMqOptionsMock;
        private readonly Mock<ILogger> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IBaseRepository<Product>> _productRepositoryMock;
        private readonly ProductService _productService;

        public GetProductTests()
        {
            _cacheServiceMock = new Mock<ICacheService>();
            _mediatorMock = new Mock<IMediator>();
            _messageProducerMock = new Mock<IMessageProducer>();
            _rabbitMqOptionsMock = new Mock<IOptions<RabbitMqSettings>>();
            _loggerMock = new Mock<ILogger>();
            _mapperMock = new Mock<IMapper>();
            _productRepositoryMock = new Mock<IBaseRepository<Product>>();

            _productService = new ProductService(
                _productRepositoryMock.Object,
                _mapperMock.Object,
                _messageProducerMock.Object,
                _rabbitMqOptionsMock.Object,
                _cacheServiceMock.Object,
                _mediatorMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnProductDtoFromCache()
        {
            //Arrange
            var productId = 1;
            var product = new ProductDto { 
                Id = productId, 
                ProductName = "Test product #1", 
                Description = "Test description #1", 
                Cost = 5000, 
                CreatedAt = DateTime.UtcNow.ToLongDateString() 
            };
            _cacheServiceMock.Setup(x => x.GetObjectAsync<ProductDto>(It.IsAny<string>()))
                .ReturnsAsync(product);

            var rabbitMqSettings = new RabbitMqSettings { RoutingKey = "test", ExchangeName = "test" };
            _rabbitMqOptionsMock.Setup(options => options.Value).Returns(rabbitMqSettings);

            // Act
            var result = await _productService.GetProductByIdAsync(productId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(productId, result.Data.Id);
            Assert.Equal(product, result.Data);
            _cacheServiceMock.Verify(x => x.GetObjectAsync<ProductDto>(It.IsAny<string>()), Times.Once);
            _mediatorMock.Verify(x => x.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()), Times.Never);
            Assert.Equal("Test product #1", result.Data.ProductName);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldFetchProductFromDatabaseAndCache()
        {
            // Arrange
            var productId = 2;
            var product = new ProductDto
            {
                Id = productId,
                ProductName = "Test product #1",
                Description = "Test description #1",
                Cost = 5000,
                CreatedAt = DateTime.UtcNow.ToLongDateString()
            };
            _cacheServiceMock.Setup(x => x.GetObjectAsync<ProductDto>(It.IsAny<string>()))
                .ReturnsAsync((ProductDto)null);
            _mediatorMock.Setup(x => x.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var rabbitMqSettings = new RabbitMqSettings { RoutingKey = "test", ExchangeName = "test" };
            _rabbitMqOptionsMock.Setup(options => options.Value).Returns(rabbitMqSettings);

            // Act
            var result = await _productService.GetProductByIdAsync(productId);

            // Assert
            Assert.Equal(product, result.Data);
            _cacheServiceMock.Verify(x => x.GetObjectAsync<ProductDto>(It.IsAny<string>()), Times.Once);
            _mediatorMock.Verify(x => x.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetProductByIdAsync_ProductNotFound_ReturnsErrorResult()
        {
            //Arrange
            var productId = 2;
            var product = new ProductDto
            {
                Id = productId,
                ProductName = "Test product #1",
                Description = "Test description #1",
                Cost = 5000,
                CreatedAt = DateTime.UtcNow.ToLongDateString()
            };

            _cacheServiceMock.Setup(x => x.GetObjectAsync<ProductDto>(It.IsAny<string>()))
                .ReturnsAsync((ProductDto)null);

            var rabbitMqSettings = new RabbitMqSettings { RoutingKey = "test", ExchangeName = "test" };
            _rabbitMqOptionsMock.Setup(options => options.Value).Returns(rabbitMqSettings);

            // Act
            var result = await _productService.GetProductByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(result.ErrorCode, (int)ErrorCodes.ProductNotFound);
        }
    }
}
