using AutoMapper;
using Microsoft.Extensions.Options;
using Moq;
using OrderPaymentSystem.Application.Services;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Interfaces.Cache;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Settings;
using OrderPaymentSystem.Producer.Interfaces;

namespace OrderPaymentSystem.UnitTests.Fixtures
{
    public class ProductServiceFixture : IDisposable
    {
        public ProductServiceFixture()
        {
            ProductRepositoryMock = new Mock<IBaseRepository<Product>>();
            MapperMock = new Mock<IMapper>();
            MessageProducerMock = new Mock<IMessageProducer>();
            RabbitMqOptionsMock = new Mock<IOptions<RabbitMqSettings>>();
            CacheServiceMock = new Mock<ICacheService>();

            var rabbitMqSettings = new RabbitMqSettings { RoutingKey = "test", ExchangeName = "test" };
            RabbitMqOptionsMock.Setup(options => options.Value).Returns(rabbitMqSettings);

            ProductService = new ProductService(
                ProductRepositoryMock.Object,
                MapperMock.Object,
                MessageProducerMock.Object,
                RabbitMqOptionsMock.Object,
                CacheServiceMock.Object);
        }

        public ProductService ProductService { get; }
        public Mock<IBaseRepository<Product>> ProductRepositoryMock { get; }
        public Mock<IMapper> MapperMock { get; }
        public Mock<IMessageProducer> MessageProducerMock { get; }
        public Mock<IOptions<RabbitMqSettings>> RabbitMqOptionsMock { get; }
        public Mock<ICacheService> CacheServiceMock { get; }

        public void Dispose() { }
    }
}
