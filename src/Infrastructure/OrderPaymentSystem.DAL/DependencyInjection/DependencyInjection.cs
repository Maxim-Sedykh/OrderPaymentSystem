using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrderPaymentSystem.Application.Interfaces.Auth;
using OrderPaymentSystem.Application.Interfaces.Cache;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Application.Interfaces.RateLimit;
using OrderPaymentSystem.Application.Settings;
using OrderPaymentSystem.DAL.Auth;
using OrderPaymentSystem.DAL.Cache;
using OrderPaymentSystem.DAL.Interceptors;
using OrderPaymentSystem.DAL.Persistence;
using OrderPaymentSystem.DAL.Persistence.Repositories;
using OrderPaymentSystem.DAL.Persistence.Repositories.Base;
using OrderPaymentSystem.DAL.RateLimit;
using OrderPaymentSystem.DAL.Resilience;
using OrderPaymentSystem.DAL.Settings;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories.Base;
using OrderPaymentSystem.Domain.Entities;
using Polly;
using StackExchange.Redis;
using Order = OrderPaymentSystem.Domain.Entities.Order;
using Role = OrderPaymentSystem.Domain.Entities.Role;

namespace OrderPaymentSystem.DAL.DependencyInjection;

/// <summary>
/// Класс для внедрения зависимостей слоя DAL в общее API
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Внедрение зависимостей слоя DAL
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgresSQL");

        services.AddSingleton<AuditInterceptor>();
        services.AddDbContext<ApplicationDbContext>(optionsBuilder =>
        {
            optionsBuilder.UseNpgsql(connectionString);
        });

        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        services.InitResilience(configuration);
        services.InitRepositories();
        services.InitUnitOfWork();

        services.InitCaching(configuration);
    }

    /// <summary>
    /// Зарегистрировать resilience-пайплайны (Polly)
    /// </summary>
    public static void InitResilience(this IServiceCollection services, IConfiguration configuration)
    {
        var resilienceSection = configuration.GetSection(ResilienceSettings.SectionName);
        var resilienceSettings = new ResilienceSettings();
        resilienceSection.Bind(resilienceSettings);
        services.AddSingleton(resilienceSettings);

        services.AddResiliencePipeline("cache", (builder, context) =>
        {
            var settings = context.ServiceProvider.GetRequiredService<ResilienceSettings>();
            var logger = context.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Resilience");
            ResiliencePipelines.ConfigureCache(builder, settings.Cache, logger);
        });

        services.AddResiliencePipeline("database", (builder, context) =>
        {
            var settings = context.ServiceProvider.GetRequiredService<ResilienceSettings>();
            var logger = context.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Resilience");
            ResiliencePipelines.ConfigureDatabase(builder, settings.Database, logger);
        });
    }

    /// <summary>
    /// Зарегистрировать репозитории
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    public static void InitRepositories(this IServiceCollection services)
    {
        var types = new List<Type>()
        {
            typeof(User),
            typeof(OrderItem),
            typeof(BasketItem),
            typeof(Order),
            typeof(Payment),
            typeof(Product),
            typeof(UserToken),
            typeof(UserRole),
            typeof(Role),
            typeof(BasketItem)
        };

        foreach (var type in types)
        {
            var interfaceType = typeof(IBaseRepository<>).MakeGenericType(type);
            var implementationType = typeof(BaseRepository<>).MakeGenericType(type);
            services.AddScoped(interfaceType, implementationType);
        }

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IBasketItemRepository, BasketItemRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IUserTokenRepository, UserTokenRepository>();
    }

    /// <summary>
    /// Зарегистрировать UnitOfWork объект
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    public static void InitUnitOfWork(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>(provider =>
            new UnitOfWork(
                provider.GetRequiredService<ApplicationDbContext>(),
                provider.GetRequiredKeyedService<ResiliencePipeline>("database"),
                () => provider.GetRequiredService<IOrderRepository>(),
                () => provider.GetRequiredService<IProductRepository>(),
                () => provider.GetRequiredService<IOrderItemRepository>(),
                () => provider.GetRequiredService<IBasketItemRepository>(),
                () => provider.GetRequiredService<IPaymentRepository>(),
                () => provider.GetRequiredService<IRoleRepository>(),
                () => provider.GetRequiredService<IUserRepository>(),
                () => provider.GetRequiredService<IUserRoleRepository>(),
                () => provider.GetRequiredService<IUserTokenRepository>()
            )
        );
    }

    /// <summary>
    /// Зарегистрировать кэширование Redis
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    /// <param name="configuration">Конфигурация</param>
    public static void InitCaching(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICacheService>(provider =>
            new RedisCacheService(
                provider.GetRequiredService<IDistributedCache>(),
                provider.GetRequiredService<ILogger<RedisCacheService>>(),
                provider.GetRequiredKeyedService<ResiliencePipeline>("cache")
            ));

        var redisConfig = configuration.GetSection(nameof(RedisSettings));
        var redisUrl = redisConfig[nameof(RedisSettings.Url)];

        if (redisUrl is null)
        {
            throw new ArgumentNullException($"{nameof(redisUrl)} was null");
        }

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisUrl;
            options.InstanceName = redisConfig[nameof(RedisSettings.InstanceName)];
        });

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            return ConnectionMultiplexer.Connect(redisUrl);
        });

        services.AddScoped<IRateLimitStore, RedisRateLimitStore>();
    }
}
