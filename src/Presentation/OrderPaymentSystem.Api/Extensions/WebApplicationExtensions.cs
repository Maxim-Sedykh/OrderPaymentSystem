using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.DAL.Persistence;

namespace OrderPaymentSystem.Api.Extensions;

/// <summary>
/// Здесь настраивается конвейер обработки (Middleware) и миграции.
/// </summary>
public static class WebApplicationExtensions
{
    public static void UseSwaggerUiConfiguration(this WebApplication app)
    {
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
    }

    public static void ApplyDatabaseMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
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
}
