using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;
using MockQueryable.Moq;
using Moq;
using OrderPaymentSystem.Application.Commands;
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
    public class CreateProductTests : IClassFixture<ProductServiceFixture>
    {
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IMessageProducer> _messageProducerMock;
        private readonly Mock<IOptions<RabbitMqSettings>> _rabbitMqOptionsMock;
        private readonly Mock<ILogger> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IBaseRepository<Product>> _productRepositoryMock;
        private readonly ProductService _productService;

        public CreateProductTests()
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
        public async Task CreateProductAsync_ValidInput_ReturnsSuccessResultWithData()
        {
            // Arrange
            var dto = new CreateProductDto("ProductForTest", "Description of product for test", 999.99m);

            _mapperMock.Setup(mapper => mapper.Map<Product>(dto)).Returns(new Product());
            _mapperMock.Setup(mapper => mapper.Map<ProductDto>(It.IsAny<Product>())).Returns(new ProductDto());
            _productRepositoryMock.Setup(repo => repo.GetAll())
                .Returns(new List<Product>().AsQueryable().BuildMockDbSet().Object);

            var rabbitMqSettings = new RabbitMqSettings { RoutingKey = "test", ExchangeName = "test" };
            _rabbitMqOptionsMock.Setup(options => options.Value).Returns(rabbitMqSettings);

            _mediatorMock.Setup(x => x.Send(It.IsAny<CreateProductCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ProductDto());

            // Act
            var result = await _productService.CreateProductAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.IsType<ProductDto>(result.Data);
            _mediatorMock.Verify(x => x.Send(It.IsAny<CreateProductCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateProductAsync_DuplicateProductName_ReturnsErrorResult()
        {
            // Arrange
            var dto = new CreateProductDto("Test Product #1", "Test description", 999.99m);
            var product = new Product
            {
                Id = 1,
                ProductName = "Test Product #1",
                Description = "Test description of product #1",
                Cost = 5000,
                CreatedAt = DateTime.UtcNow
            };

            _productRepositoryMock.Setup(repo => repo.GetAll())
                .Returns(new List<Product>() { product }.AsQueryable().BuildMockDbSet().Object);
            _mapperMock.Setup(mapper => mapper.Map<Product>(dto)).Returns(new Product());

            // Act
            var result = await _productService.CreateProductAsync(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal((int)ErrorCodes.ProductAlreadyExist, result.ErrorCode);
        }
    }
}
