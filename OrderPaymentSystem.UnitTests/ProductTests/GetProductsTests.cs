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
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IMessageProducer> _messageProducerMock;
        private readonly Mock<IOptions<RabbitMqSettings>> _rabbitMqOptionsMock;
        private readonly Mock<ILogger> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IBaseRepository<Product>> _productRepositoryMock;
        private readonly ProductService _productService;

        public GetProductsTests()
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
        public async Task GetProductsAsync_ProductsFound_ShouldReturnProductDtosFromCache()
        {
            var products = new ProductDto[]
            {
                new ProductDto
                {
                    Id = 1,
                    ProductName = "Test Product #1",
                    Description = "Test description of product #1",
                    Cost = 5000,
                    CreatedAt = DateTime.UtcNow.ToLongDateString(),
                },
                new ProductDto
                {
                    Id = 2,
                    ProductName = "Test Product #2",
                    Description = "Test description of product #2",
                    Cost = 1500,
                    CreatedAt = DateTime.UtcNow.ToLongDateString(),
                },
            };

            _cacheServiceMock.Setup(x => x.GetObjectAsync<ProductDto[]>(It.IsAny<string>()))
                .ReturnsAsync(products);

            var rabbitMqSettings = new RabbitMqSettings { RoutingKey = "test", ExchangeName = "test" };
            _rabbitMqOptionsMock.Setup(options => options.Value).Returns(rabbitMqSettings);

            // Act
            var result = await _productService.GetProductsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task GetProductsAsync_ShouldFetchProductsFromDatabaseAndCache()
        {
            // Arrange
            var products = new ProductDto[]
            {
                new ProductDto
                {
                    Id = 1,
                    ProductName = "Test Product #1",
                    Description = "Test description of product #1",
                    Cost = 5000,
                    CreatedAt = DateTime.UtcNow.ToLongDateString(),
                },
                new ProductDto
                {
                    Id = 2,
                    ProductName = "Test Product #2",
                    Description = "Test description of product #2",
                    Cost = 1500,
                    CreatedAt = DateTime.UtcNow.ToLongDateString(),
                },
            };
            _cacheServiceMock.Setup(x => x.GetObjectAsync<ProductDto[]>(It.IsAny<string>()))
                .ReturnsAsync((ProductDto[])null);
            _mediatorMock.Setup(x => x.Send(It.IsAny<GetProductsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            var rabbitMqSettings = new RabbitMqSettings { RoutingKey = "test", ExchangeName = "test" };
            _rabbitMqOptionsMock.Setup(options => options.Value).Returns(rabbitMqSettings);

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
        public async Task GetProducts_ProductsNotFound_ReturnsErrorResult()
        {
            //Arrange
            _cacheServiceMock.Setup(x => x.GetObjectAsync<ProductDto[]>(It.IsAny<string>()))
                .ReturnsAsync((ProductDto[])null);

            var rabbitMqSettings = new RabbitMqSettings { RoutingKey = "test", ExchangeName = "test" };
            _rabbitMqOptionsMock.Setup(options => options.Value).Returns(rabbitMqSettings);

            // Act
            var result = await _productService.GetProductsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(result.ErrorCode, (int)ErrorCodes.ProductsNotFound);
        }
    }
}
