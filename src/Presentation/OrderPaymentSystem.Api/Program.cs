using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.Api.Extensions;
using OrderPaymentSystem.Api.Middlewares;
using Prometheus;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogConfiguration();

builder.Services.AddApiInfrastructure(builder.Configuration);
builder.Services.AddAuthConfiguration(builder.Configuration);
builder.Services.AddSwaggerConfiguration();

    var app = builder.Build();

    app.UseSwaggerUiConfiguration();
    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseSerilogRequestLogging();

    app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
    app.UseHttpsRedirection();

    app.UseMetricServer();
    app.UseHttpMetrics();

    app.UseAuthentication();
    app.UseAuthorization();

    await app.ApplyDatabaseMigrations();

    app.MapGet("/ping", () => Results.Ok("pong"));
    app.MapMetrics();
    app.MapControllers();

    app.Run();

public partial class Program { }

public interface IApiMarker { }

