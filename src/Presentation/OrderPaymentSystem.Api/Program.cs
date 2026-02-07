using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.Api;
using OrderPaymentSystem.Api.Middlewares;
using OrderPaymentSystem.Application.DependencyInjection;
using OrderPaymentSystem.Application.Settings;
using OrderPaymentSystem.Application.Validations.FluentValidations.Auth;
using OrderPaymentSystem.DAL.DependencyInjection;
using OrderPaymentSystem.DAL.Persistence;
using OrderPaymentSystem.Domain.Settings;
using Prometheus;
using Serilog;
using Serilog.Debugging;

var builder = WebApplication.CreateBuilder(args); //TODO сделать почище program.cs

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(nameof(JwtSettings)));
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection(nameof(RedisSettings)));

builder.Services.AddEndpointsApiExplorer();
builder.Services.UseHttpClientMetrics();

builder.Services.AddControllers();

builder.Services.AddValidatorsFromAssemblyContaining<RegisterUserValidator>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddAuthenticationAndAuthorization(builder);
builder.Services.AddSwagger();

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()

    .WriteTo.Console()
    .WriteTo.Elasticsearch([new Uri(builder.Configuration["ElasticConfiguration:Uri"])], opts =>
    {
        opts.DataStream = new DataStreamName("logs", "orderpaymentsystem-api", "dev");
        opts.BootstrapMethod = BootstrapMethod.Silent;
    })
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddApplication();

var app = builder.Build();

app.UseSerilogRequestLogging();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ошибка при применении миграций БД.");
    }
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "OrderPaymentSystem Swagger v1.0");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "OrderPaymentSystem Swagger v2.0");
        c.RoutePrefix = string.Empty;
    });
}

app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseHttpsRedirection();

app.UseMetricServer();
app.UseHttpMetrics();

app.MapGet("/ping", () => Results.Ok());

app.UseAuthorization();

app.MapMetrics();
app.MapControllers();

app.Run();
