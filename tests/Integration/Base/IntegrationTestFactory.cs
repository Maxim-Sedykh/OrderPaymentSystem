using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrderPaymentSystem.Application.DTOs.Product;
using OrderPaymentSystem.Application.Interfaces.Cache;
using OrderPaymentSystem.DAL.Cache;
using OrderPaymentSystem.DAL.Persistence;
using OrderPaymentSystem.IntegrationTests.Constants;
using System.Net.Http.Json;
using Testcontainers.Elasticsearch;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace OrderPaymentSystem.IntegrationTests.Base;

/// <summary>
/// Фабрика для создания всех зависимостей в интеграционном тесте.
/// </summary>
public class IntegrationTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    /// <summary>
    /// Тестовый Docker-контейнер PostgreSQL
    /// </summary>
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder(ContainerImages.Postgres)
        .WithEnvironment("PARENT_PROCESS_ID", "")
        .WithDatabase("OrderPaymentSystemTest")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .WithCleanUp(true)
        .Build();

    /// <summary>
    /// Тестовый Docker-контейнер Redis
    /// </summary>
    private readonly RedisContainer _redisContainer = new RedisBuilder(ContainerImages.Redis)
        .WithCleanUp(true)
        .Build();

    /// <summary>
    /// Тестовый Docker-контейнер ElasticSearch
    /// </summary>
    private readonly ElasticsearchContainer _esContainer = new ElasticsearchBuilder(ContainerImages.Elasticsearch)
            .WithCleanUp(true)
            .Build();

    /// <inheritdoc/>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.RemoveAll<ApplicationDbContext>();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(_dbContainer.GetConnectionString());
            });

            services.RemoveAll<ICacheService>();
            services.RemoveAll<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = _redisContainer.GetConnectionString();
                options.InstanceName = "OrderPaymentSystemTest";
            });

            services.AddScoped<ICacheService, RedisCacheService>();
        });
    }

    /// <summary>
    /// Метод, вызываемый перед началом всех тестов.
    /// Используется для запуска Docker-контейнеров.
    /// </summary>
    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _redisContainer.StartAsync();
        await _esContainer.StartAsync();

        Environment.SetEnvironmentVariable("ConnectionStrings__PostgresSQL", _dbContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("ElasticConfiguration__Uri", _esContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("RedisSettings__Url", _redisContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("RedisSettings__InstanceName", "OrderPaymentSystemTest");
        Environment.SetEnvironmentVariable("JwtSettings__JwtKey", "7f3a5b8c2d1e4f9a0b6c3d8e5f2a1b4c7d9e0f2b5a8c1d4e7f9b0a3c6d2e5f8b");
        Environment.SetEnvironmentVariable("AdminSettings__Login", TestConstants.AdminLogin);
        Environment.SetEnvironmentVariable("AdminSettings__Password", TestConstants.AdminPassword);
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");

        var client = CreateClient();
    }

    /// <summary>
    /// Метод, вызываемый после завершения всех тестов.
    /// Используется для остановки и удаления Docker-контейнеров.
    /// </summary>
    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _redisContainer.StopAsync();
        await _esContainer.StopAsync();

        await base.DisposeAsync();
    }

    /// <summary>
    /// Создать товар
    /// </summary>
    /// <param name="productDto">Модель создания товара</param>
    /// <param name="client">Клиент для http запроса на создание</param>
    /// <returns><see cref="ProductDto"/> или null</returns>
    public async Task<ProductDto?> CreateProductAsync(CreateProductDto productDto, HttpClient client)
    {
        var response = await client.PostAsJsonAsync(TestConstants.ApiProductsV1, productDto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ProductDto>();
    }
}
