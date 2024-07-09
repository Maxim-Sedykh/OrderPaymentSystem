using OrderPaymentSystem.DAL.DependencyInjection;
using OrderPaymentSystem.Application.DependencyInjection;
using Serilog;
using OrderPaymentSystem.Api;
using OrderPaymentSystem.Domain.Settings;
using OrderPaymentSystem.Api.Middlewares;
using OrderPaymentSystem.Consumer.DependencyInjection;
using OrderPaymentSystem.Producer.DependencyInjection;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);   

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection(nameof(RabbitMqSettings)));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.DefaultSection));
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection(nameof(RedisSettings)));

builder.Services.AddEndpointsApiExplorer();
builder.Services.UseHttpClientMetrics();

builder.Services.AddControllers();

builder.Services.AddAuthenticationAndAuthorization(builder);
builder.Services.AddSwagger();
builder.Services.AddHttpContextAccessor();

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddProducer();
builder.Services.AddConsumer();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
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

app.MapGet("/test-random-number", () =>
{
    var number = Random.Shared.Next(0, 100);
    return Results.Ok(number);
}
);

app.UseAuthorization();

app.MapMetrics();
app.MapControllers();

app.Run();
