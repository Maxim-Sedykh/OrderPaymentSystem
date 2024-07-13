﻿using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using OrderPaymentSystem.Application.Services;
using OrderPaymentSystem.Domain.Dto.Product;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Interfaces.Cache;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Settings;
using OrderPaymentSystem.Producer.Interfaces;
using Serilog;

namespace OrderPaymentSystem.UnitTests.Configurations
{
    public class ProductServiceFixture : IDisposable
    {
        public ProductServiceFixture()
        {
            ProductRepositoryMock = MockRepositoriesGetter.GetMockProductRepository();
            MapperMock = new Mock<IMapper>();
            MessageProducerMock = new Mock<IMessageProducer>();
            RabbitMqOptionsMock = new Mock<IOptions<RabbitMqSettings>>();
            CacheServiceMock = new Mock<ICacheService>();
            MediatorMock = new Mock<IMediator>();
            ProductRepositoryMock = MockRepositoriesGetter.GetMockProductRepository();
            LoggerMock = new Mock<ILogger>();

            var rabbitMqSettings = new RabbitMqSettings { RoutingKey = "test", ExchangeName = "test" };
            RabbitMqOptionsMock.Setup(options => options.Value).Returns(rabbitMqSettings);

            ProductService = new ProductService(
                ProductRepositoryMock.Object,
                MapperMock.Object,
                MessageProducerMock.Object,
                RabbitMqOptionsMock.Object,
                CacheServiceMock.Object,
                MediatorMock.Object,
                LoggerMock.Object);
        }

        public ProductService ProductService { get; }
        public Mock<IBaseRepository<Product>> ProductRepositoryMock { get; }
        public Mock<IMapper> MapperMock { get; }
        public Mock<IMessageProducer> MessageProducerMock { get; }
        public Mock<IOptions<RabbitMqSettings>> RabbitMqOptionsMock { get; }
        public Mock<ICacheService> CacheServiceMock { get; }
        public Mock<IMediator> MediatorMock { get; }
        public Mock<ILogger> LoggerMock { get; }

        public ProductDto[] GetProductDtos() => new ProductDto[]
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

        public ProductDto GetProductDto() => new ProductDto
        {
            Id = 1,
            ProductName = "Test product #1",
            Description = "Test description #1",
            Cost = 5000,
            CreatedAt = DateTime.UtcNow.ToLongDateString()
        };

        public void Dispose() { }
    }
}
