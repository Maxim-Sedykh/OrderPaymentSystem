using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderPaymentSystem.DAL.Auth;
using OrderPaymentSystem.DAL.Cache;
using OrderPaymentSystem.DAL.Interceptors;
using OrderPaymentSystem.DAL.Repositories.Base;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Interfaces.Auth;
using OrderPaymentSystem.Domain.Interfaces.Cache;
using OrderPaymentSystem.Domain.Interfaces.Databases;
using OrderPaymentSystem.Domain.Interfaces.Databases.Repositories.Base;
using OrderPaymentSystem.Domain.Settings;

namespace OrderPaymentSystem.DAL.DependencyInjection;

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
            optionsBuilder.UseNpgsql(connectionString, options => options.EnableRetryOnFailure());
        });

        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        services.InitRepositories();
        services.InitUnitOfWork();

        services.InitCaching(configuration);
    }

    private static void InitRepositories(this IServiceCollection services)
    {
        var types = new List<Type>()
        {
            typeof(User),
            typeof(OrderItem),
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
    }

    private static void InitUnitOfWork(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    private static void InitCaching(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICacheService, DistributedCacheService>();

        var redisConfig = configuration.GetSection(nameof(RedisSettings));
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConfig[nameof(RedisSettings.Url)];
            options.InstanceName = redisConfig[nameof(RedisSettings.InstanceName)];
        });
    }
}
