using BenchmarkDotNet.Attributes;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using OrderPaymentSystem.Application.Settings;
using OrderPaymentSystem.DAL.DependencyInjection;
using OrderPaymentSystem.DAL.Persistence;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace OrderPaymentSystem.Benchmarks.Base;

/// <summary>
/// Базовый класс для бенчмарков, с базовыми настройками для каждого бенчмарка.
/// </summary>
[Config(typeof(BenchmarkerConfig))]
[MemoryDiagnoser]
[Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public abstract class BaseBenchmark
{
    /// <summary>
    /// Тестовый контейнер PostgreSQL
    /// </summary>
    private static readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder("postgres:15-alpine")
        .WithDatabase("benchmark_db")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    /// <summary>
    /// Тестовый контейнер Redis
    /// </summary>
    private static readonly RedisContainer _redisContainer = new RedisBuilder("redis:alpine")
        .Build();

    /// <summary>
    /// ServiceProvider. Для получения объекта из DI.
    /// </summary>
    protected IServiceProvider? ServiceProvider { get; private set; }

    /// <summary>
    /// Контекст объектов. Их область видимости.
    /// </summary>
    protected IServiceScope? ServiceScope { get; private set; }

    /// <summary>
    /// Настройка зависимостей.
    /// </summary>
    [GlobalSetup]
    public virtual async Task GlobalSetup()
    {
        await _postgreSqlContainer.StartAsync();
        await _redisContainer.StartAsync();

        var services = new ServiceCollection();

        var jwtSettings = new JwtSettings
        {
            AccessTokenValidityInMinutes = 60,
            RefreshTokenValidityInDays = 7,
            Issuer = "test-issuer",
            Audience = "test-audience",
            JwtKey = "supersecretkeythatisatleast256bitslong"
        };

        services.AddSingleton(Options.Create(jwtSettings));
        services.AddSingleton(Options.Create(new AdminSettings { Login = "admin", Password = "123" }));
        services.AddSingleton(TimeProvider.System);

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(_postgreSqlContainer.GetConnectionString()));

        services.AddStackExchangeRedisCache(options =>
            options.Configuration = _redisContainer.GetConnectionString());

        services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
        services.AddScoped<IMapper, ServiceMapper>();
        services.AddSingleton(TypeAdapterConfig.GlobalSettings);

        services.InitRepositories();
        services.InitUnitOfWork();

        RegisterServices(services);

        ServiceProvider = services.BuildServiceProvider();
        ServiceScope = ServiceProvider.CreateScope();

        var db = ServiceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
    }

    /// <summary>
    /// Зарегистрировать специфичные для определённого бенчмарка сервисы
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    protected abstract void RegisterServices(IServiceCollection services);

    /// <summary>
    /// Очистка внешних ресурсов бенчмарка
    /// </summary>
    [GlobalCleanup]
    public virtual async Task GlobalCleanup()
    {
        ServiceScope?.Dispose();

        await _postgreSqlContainer.StopAsync();
        await _redisContainer.StopAsync();
    }
}
