using Hangfire;
using Hangfire.PostgreSql;
using OrderPaymentSystem.Api.Jobs;

namespace OrderPaymentSystem.Api.Extensions;

public static class HangfireJobExtensions
{
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
            "0 3 * * *"
        );

        return app;
    }
}
