using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.Api.Extensions;
using OrderPaymentSystem.Api.Middlewares;
using Prometheus;
using Serilog;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.AddSerilogConfiguration();

    builder.Services.AddApiInfrastructure(builder.Configuration);
    builder.Services.AddAuthConfiguration(builder.Configuration);
    builder.Services.AddSwaggerConfiguration();

    builder.Services.AddHealthChecksConfiguration(builder.Configuration);

    builder.Services.AddHangfireJobs(builder.Configuration);

    var app = builder.Build();

    app.UseSwaggerUiConfiguration();
    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseSerilogRequestLogging();

    app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
    app.UseHttpsRedirection();

    app.UseHealthChecksConfiguration();

    if (!app.Environment.IsEnvironment("Test"))
    {
        app.UseMetricServer();
        app.UseHttpMetrics();
        app.UseHangfireJobsConfig();
    }

    app.UseAuthentication();
    app.UseAuthorization();

    await app.ApplyDatabaseMigrationsAsync();

    app.MapMetrics();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "The application failed to start correctly");
}
finally
{
    Log.CloseAndFlush();
}

