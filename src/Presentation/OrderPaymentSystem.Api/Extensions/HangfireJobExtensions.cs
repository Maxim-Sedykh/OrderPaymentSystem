using Hangfire;
using Hangfire.PostgreSql;
using OrderPaymentSystem.Api.Jobs;

namespace OrderPaymentSystem.Api.Extensions;

/// <summary>
/// Расширения для добавления фоновых задач Hangfire
/// </summary>
public static class HangfireJobExtensions
{
    /// <summary>
    /// Зарегистрировать фоновые задачи HangFire
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    /// <param name="configuration">Конфиг</param>
    public static IServiceCollection AddHangfireJobs(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgresSQL");

        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(options => options.UseNpgsqlConnection(connectionString)));

        services.AddHangfireServer();

        services.AddScoped<OrderJobs>();
        services.AddScoped<AuthJobs>();

        return services;
    }

    /// <summary>
    /// Добавить и настроить конкретные фоновые задачи
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseHangfireJobsConfig(this IApplicationBuilder app)
    {
        app.UseHangfireDashboard("/hangfire");

        RecurringJob.AddOrUpdate<OrderJobs>(
            "cancel-expired-orders",
            job => job.CancelExpiredOrders(CancellationToken.None),
            Cron.Daily
        );

        RecurringJob.AddOrUpdate<AuthJobs>(
            "cleanup-expired-tokens",
            job => job.CleanupTokens(CancellationToken.None),
            Cron.Weekly
        );

        return app;
    }
}
