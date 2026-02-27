using BenchmarkDotNet.Attributes;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrderPaymentSystem.Application.Interfaces.Cache;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Application.Interfaces.Services;
using OrderPaymentSystem.Application.Services.Products;
using OrderPaymentSystem.Benchmarks.Base;
using OrderPaymentSystem.Benchmarks.Mocks;
using OrderPaymentSystem.DAL.Cache;
using OrderPaymentSystem.DAL.Persistence;
using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.Benchmarks.Benchmarks;

/// <summary>
/// Бенчмарк <see cref="ProductService"/>.
/// Замер производительности.
/// </summary>
[Config(typeof(BenchmarkerConfig))]
[MemoryDiagnoser]
public class ProductServiceBenchmarks : BaseBenchmark
{
    /// <summary>
    /// Сервис для работы с товарами с поддержкой кэша
    /// </summary>
    private ProductService? _productServiceWithCache;

    /// <summary>
    /// Сервис для работы с товарами без кэша
    /// </summary>
    private ProductService? _productServiceNoCache;

    /// <summary>
    /// Контекст для работы с БД
    /// </summary>
    private ApplicationDbContext? _dbContext;

    /// <summary>
    /// Количество товаров которое будем получать
    /// </summary>
    private const int ProductCount = 10000;

    /// <inheritdoc/>
    protected override void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<ICacheService, RedisCacheService>();
        services.AddScoped<IProductService, ProductService>();
    }

    /// <inheritdoc/>
    public override async Task GlobalSetup()
    {
        await base.GlobalSetup();

        _dbContext = ServiceScope!.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (!await _dbContext.Products.AnyAsync())
        {
            var products = Enumerable.Range(1, ProductCount)
                .Select(i => Product.Create($"Product {i}", $"Description {i}", (decimal)(i * 10 + 10), 100))
                .ToList();
            _dbContext.Products.AddRange(products);
            await _dbContext.SaveChangesAsync();
        }

        var unitOfWork = ServiceScope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var mapper = ServiceScope.ServiceProvider.GetRequiredService<IMapper>();
        var logger = ServiceScope.ServiceProvider.GetRequiredService<ILogger<ProductService>>();

        var cacheService = ServiceScope.ServiceProvider.GetRequiredService<ICacheService>();

        var noCacheService = new NoCacheService();

        _productServiceWithCache = new ProductService(unitOfWork, logger, cacheService, mapper);
        _productServiceNoCache = new ProductService(unitOfWork, logger, noCacheService, mapper);

        await _productServiceWithCache.GetAllAsync();
    }

    /// <summary>
    /// Измерить скорость и производительность метода GetAllAsync в <see cref="ProductService"/>
    /// C использованием кэша
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Product_Read")]
    public async Task GetAll_WithRealCache()
    {
        await _productServiceWithCache!.GetAllAsync();
        _dbContext!.ChangeTracker.Clear();
    }

    /// <summary>
    /// Измерить скорость и производительность метода GetAllAsync в <see cref="ProductService"/>
    /// Без кэша
    /// </summary>
    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Product_Read")]
    public async Task GetAll_NoCache()
    {
        await _productServiceNoCache!.GetAllAsync();
        _dbContext!.ChangeTracker.Clear();
    }
}