using AutoMapper;
using MediatR;
using Moq;
using OrderPaymentSystem.Application.DTOs.Product;
using OrderPaymentSystem.Application.Interfaces.Cache;
using OrderPaymentSystem.Application.Services;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Interfaces.Repositories.Base;
using Serilog;

namespace OrderPaymentSystem.UnitTests.Configurations;

public class ProductServiceFixture : IDisposable
{
    public ProductServiceFixture()
    {
        ProductRepositoryMock = MockRepositoriesGetter.GetMockProductRepository();
        MapperMock = new Mock<IMapper>();
        CacheServiceMock = new Mock<ICacheService>();
        MediatorMock = new Mock<IMediator>();
        ProductRepositoryMock = MockRepositoriesGetter.GetMockProductRepository();
        LoggerMock = new Mock<ILogger>();

        ProductService = new ProductService(
            ProductRepositoryMock.Object,
            MapperMock.Object,
            MediatorMock.Object,
            LoggerMock.Object);
    }

    public ProductService ProductService { get; }
    public Mock<IBaseRepository<Product>> ProductRepositoryMock { get; }
    public Mock<IMapper> MapperMock { get; }
    public Mock<ICacheService> CacheServiceMock { get; }
    public Mock<IMediator> MediatorMock { get; }
    public Mock<ILogger> LoggerMock { get; }

    public ProductDto[] GetProductDtos() => new ProductDto[]
    {
        new() {
            Id = 1,
            ProductName = "Test Product #1",
            Description = "Test description of product #1",
            Cost = 5000,
            CreatedAt = DateTime.UtcNow.ToLongDateString(),
        },
        new() {
            Id = 2,
            ProductName = "Test Product #2",
            Description = "Test description of product #2",
            Cost = 1500,
            CreatedAt = DateTime.UtcNow.ToLongDateString(),
        },
    };

    public ProductDto GetProductDto() => new()
    {
        Id = 1,
        ProductName = "Test product #1",
        Description = "Test description #1",
        Cost = 5000,
        CreatedAt = DateTime.UtcNow.ToLongDateString()
    };

    public void Dispose() { }
}
