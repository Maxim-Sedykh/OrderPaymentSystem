using OrderPaymentSystem.DAL.DependencyInjection;
using OrderPaymentSystem.Application.DependencyInjection;
using Serilog;
using OrderPaymentSystem.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSwagger();

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddApplication();

var app = builder.Build();

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
