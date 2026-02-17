using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.Application.Interfaces.Auth;
using OrderPaymentSystem.DAL.Persistence;
using OrderPaymentSystem.Domain.Entities;
using System;

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

    public static async Task ApplyDatabaseMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var passwordHasher = services.GetRequiredService<IPasswordHasher>();
            if (context.Database.GetPendingMigrations().Any())
            {
                await context.Database.MigrateAsync();
            }
            await EnsureCreatedRole(context, "User");
            await EnsureCreatedRole(context, "Moderator");
            var adminRole = await EnsureCreatedRole(context, "Admin");

            if (!await context.Users.AnyAsync(r => r.Login == "admin"))
            {

                var admin = User.Create("admin", passwordHasher.Hash("12345"));
                admin.AddRoles(adminRole);

                await context.Users.AddAsync(admin);

                await context.SaveChangesAsync();
            }

        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Ошибка при применении миграций БД.");
        }
    }



    private static async Task<Role> EnsureCreatedRole(ApplicationDbContext context, string roleName)
    {
        if (!await context.Roles.AnyAsync(r => r.Name == roleName))
        {
            var userRole = Role.Create(roleName);
            context.Roles.Add(userRole);

            await context.SaveChangesAsync();

            return userRole;
        }

        return null;
    }
}
