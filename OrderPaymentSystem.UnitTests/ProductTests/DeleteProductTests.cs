using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MockQueryable.Moq;
using Moq;
using OrderPaymentSystem.Application.Commands;
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
    public class DeleteProductTests : IClassFixture<ProductServiceFixture>
    {
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IMessageProducer> _messageProducerMock;
        private readonly Mock<IOptions<RabbitMqSettings>> _rabbitMqOptionsMock;
        private readonly Mock<ILogger> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IBaseRepository<Product>> _productRepositoryMock;
        private readonly ProductService _productService;

        public DeleteProductTests()
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
        public async Task DeleteProductAsync_ShouldDeleteProductAndSendMessage()
        {
            // Arrange
            var productId = 2;
            var product = new Product { Id = productId };
            var productDto = new ProductDto { Id = productId };
            _productRepositoryMock.Setup(repo => repo.GetAll())
                .Returns(new List<Product>() { product }.AsQueryable().BuildMockDbSet().Object);
            _mapperMock.Setup(x => x.Map<ProductDto>(product)).Returns(productDto);
            var rabbitMqSettings = new RabbitMqSettings { RoutingKey = "test", ExchangeName = "test" };
            _rabbitMqOptionsMock.Setup(options => options.Value).Returns(rabbitMqSettings);

            // Act
            var result = await _productService.DeleteProductAsync(productId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(productDto, result.Data);
            _mediatorMock.Verify(x => x.Send(It.Is<DeleteProductCommand>(cmd => cmd.Product.Id == productId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteProductAsync_NonExistingProduct_ReturnsErrorResult()
        {
            // Arrange
            var productId = 0;

            _productRepositoryMock.Setup(repo => repo.GetAll())
                .Returns(new List<Product>().AsQueryable().BuildMockDbSet().Object);

            var rabbitMqSettings = new RabbitMqSettings { RoutingKey = "test", ExchangeName = "test" };
            _rabbitMqOptionsMock.Setup(options => options.Value).Returns(rabbitMqSettings);

            // Act
            var result = await _productService.DeleteProductAsync(productId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal((int)ErrorCodes.ProductNotFound, result.ErrorCode);
        }
    }
}
