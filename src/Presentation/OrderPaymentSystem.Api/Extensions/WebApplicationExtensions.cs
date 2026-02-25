using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrderPaymentSystem.Application.Interfaces.Auth;
using OrderPaymentSystem.Application.Settings;
using OrderPaymentSystem.DAL.Persistence;
using OrderPaymentSystem.Domain.Constants;
using OrderPaymentSystem.Domain.Entities;
using System.Text.Json;

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

    public static async Task ApplyDatabaseMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();

            if ((await context.Database.GetPendingMigrationsAsync()).Any())
            {
                await context.Database.MigrateAsync();
            }

            await SeedIdentityDataAsync(services);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Error while migrating database");

            throw;
        }
    }

    private static async Task SeedIdentityDataAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = services.GetRequiredService<IPasswordHasher>();
        var adminSettings = services.GetRequiredService<IOptions<AdminSettings>>().Value;

        await EnsureCreatedRole(context, DefaultRoles.User);
        await EnsureCreatedRole(context, DefaultRoles.Moderator);
        var adminRole = await EnsureCreatedRole(context, DefaultRoles.Admin);

        await EnsureAdminUser(context, passwordHasher, adminRole, adminSettings);
    }


    private static async Task<Role> EnsureCreatedRole(ApplicationDbContext context, string roleName)
    {
        var role = await context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);

        if (role == null)
        {
            role = Role.Create(roleName);
            context.Roles.Add(role);
            await context.SaveChangesAsync();
        }

        return role;
    }

    private static async Task EnsureAdminUser(ApplicationDbContext context, IPasswordHasher passwordHasher, Role adminRole, AdminSettings adminSettings)
    {
        if (await context.Users.AnyAsync(u => u.Login == adminSettings.Login))
            return;

        var admin = User.Create(adminSettings.Login, passwordHasher.Hash(adminSettings.Password));
        var userRole = UserRole.Create(admin.Id, adminRole.Id);

        context.Users.Add(admin);
        context.UserRoles.Add(userRole);

        await context.SaveChangesAsync();
    }

    public static void UseHealthChecksConfiguration(this WebApplication app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";

                var response = new
                {
                    status = report.Status.ToString(),
                    totalDuration = report.TotalDuration.TotalMilliseconds.ToString("F2") + "ms",
                    checks = report.Entries.Select(x => new
                    {
                        component = x.Key,
                        status = x.Value.Status.ToString(),
                        description = x.Value.Description,
                        duration = x.Value.Duration.TotalMilliseconds.ToString("F2") + "ms",
                        error = x.Value.Exception?.Message
                    })
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
                {
                    WriteIndented = true
                }));
            }
        });
    }
}
