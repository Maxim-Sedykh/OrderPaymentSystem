using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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

public class IntegrationTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    // Конфигурация PostgreSQL контейнера
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder(ContainerImages.Postgres)
        .WithEnvironment("PARENT_PROCESS_ID", "")
        .WithDatabase("OrderPaymentSystemTest") // Имя тестовой базы данных
        .WithUsername("postgres") // Имя пользователя для подключения
        .WithPassword("postgres") // Пароль для подключения
        .WithCleanUp(true) // Автоматически удалять контейнер после тестов
        .Build();

    // Конфигурация Redis контейнера
    private readonly RedisContainer _redisContainer = new RedisBuilder(ContainerImages.Redis)
        .WithCleanUp(true) // Автоматически удалять контейнер после тестов
        .Build();

    private readonly ElasticsearchContainer _esContainer = new ElasticsearchBuilder(ContainerImages.Elasticsearch)
            .WithCleanUp(true)
            .Build();

    /// <summary>
    /// Переопределяем метод для настройки хоста и сервисов.
    /// Здесь происходит подмена зависимостей для интеграционных тестов.
    /// </summary>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        builder.ConfigureAppConfiguration(config =>
        {
            // Здесь мы можем добавить или переопределить ключи конфигурации
            // Важно: AddInMemoryCollection добавляет в конец, переопределяя предыдущие значения
            config.AddInMemoryCollection(new Dictionary<string, string>
            {
                // Подменяем строку подключения к PostgreSQL
                ["ConnectionStrings:PostgresSQL"] = _dbContainer.GetConnectionString(),
                ["Elasticsearch:Url"] = _esContainer.GetConnectionString(),
                // Подменяем настройки Redis (если они читаются как URL)
                ["RedisSettings:Url"] = _redisContainer.GetConnectionString(),
                ["RedisSettings:InstanceName"] = "OrderPaymentSystemTest",
                ["JwtSettings:JwtKey"] = "7f3a5b8c2d1e4f9a0b6c3d8e5f2a1b4c7d9e0f2b5a8c1d4e7f9b0a3c6d2e5f8b",
                ["AdminSettings:Login"] = TestConstants.AdminLogin,
                ["AdminSettings:Password"] = TestConstants.AdminPassword
            }!);
        });

        // ConfigureTestServices позволяет переопределить регистрации сервисов,
        // которые были сделаны в ConfigureServices вашего приложения.
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
                // Эти options будут уже заполнены из IConfiguration.
                // Мы можем оставить их пустыми, или проставить значения по умолчанию,
                // но они будут перезаписаны IConfiguration.
                // Если InstanceName не будет переопределен в IConfiguration,
                // то он возьмется отсюда.
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
        // Запускаем PostgreSQL контейнер
        await _dbContainer.StartAsync();
        await _redisContainer.StartAsync();
        await _esContainer.StartAsync();

        // 2. ПРИНУДИТЕЛЬНО запускаем сервер через создание клиента
        // Это инициализирует сервер до того, как мы полезем в Services
        var client = CreateClient();
    }

    /// <summary>
    /// Метод, вызываемый после завершения всех тестов.
    /// Используется для остановки и удаления Docker-контейнеров.
    /// </summary>
    public new async Task DisposeAsync()
    {
        // Останавливаем и удаляем PostgreSQL контейнер
        await _dbContainer.StopAsync();
        // Останавливаем и удаляем Redis контейнер
        await _redisContainer.StopAsync();
        await _esContainer.StopAsync();

        // Не забываем освободить ресурсы WebApplicationFactory
        await base.DisposeAsync();
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto productDto, HttpClient client)
    {
        var response = await client.PostAsJsonAsync(TestConstants.ApiProductsV1, productDto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ProductDto>();
    }
}
