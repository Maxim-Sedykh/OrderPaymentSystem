using OrderPaymentSystem.Api.Middlewares;

namespace OrderPaymentSystem.Api.Extensions;

/// <summary>
/// Расширения для Rate-лимитера.
/// </summary>
public static class RateLimitMiddlewareExtensions
{
    /// <summary>
    /// Использовать Middleware для ограничения количества запросов пользователем.
    /// </summary>
    /// <param name="app">Билдер приложения.</param>
    public static IApplicationBuilder UseRateLimit(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RateLimitMiddleware>();
    }
}
