using Microsoft.Extensions.Options;
using OrderPaymentSystem.Api.Settings;
using OrderPaymentSystem.Application.Interfaces.RateLimit;
using System.Net;

namespace OrderPaymentSystem.Api.Middlewares;

/// <summary>
/// Middleware для реализации rate limiting с использованием sliding window алгоритма.
/// Хранит состояние в Redis для работы в распределённой среде.
/// </summary>
public sealed class RateLimitMiddleware(
    RequestDelegate next,
    IOptionsMonitor<RateLimitSettings> settings,
    ILogger<RateLimitMiddleware> logger,
    IRateLimitStore rateLimitStore)
{
    private readonly RequestDelegate _next = next;
    private readonly IOptionsMonitor<RateLimitSettings> _settings = settings;
    private readonly ILogger<RateLimitMiddleware> _logger = logger;
    private readonly IRateLimitStore _rateLimitStore = rateLimitStore;

    /// <summary>
    /// Исполнить логику мидлвара.
    /// </summary>
    /// <param name="context">Http контекст запроса.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        var settings = _settings.CurrentValue;

        if (!settings.Enabled)
        {
            await _next(context);
            return;
        }

        var path = context.Request.Path.Value ?? string.Empty;

        if (settings.ExcludedPaths.Any(excluded => path.StartsWith(excluded, StringComparison.OrdinalIgnoreCase)))
        {
            await _next(context);
            return;
        }

        var clientId = GetClientId(context);

        var (maxRequests, windowSize) = GetLimitsForClient(context, settings);

        var (requestCount, allowed) = await _rateLimitStore.CheckAndAddRequestAsync(
            clientId,
            windowSize,
            maxRequests);

        var currentTime = DateTime.UtcNow;

        if (!allowed)
        {
            var retryAfter = CalculateRetryAfter(windowSize);

            _logger.LogWarning(
                "Rate limit exceeded for client {ClientId}. Requests: {Requests}/{MaxRequests}. Retry after: {RetryAfter}s",
                clientId,
                requestCount,
                maxRequests,
                retryAfter);

            context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            context.Response.Headers.Append("X-RateLimit-Limit", maxRequests.ToString());
            context.Response.Headers.Append("X-RateLimit-Remaining", "0");
            context.Response.Headers.Append("X-RateLimit-Reset", currentTime.AddSeconds(retryAfter).ToString("r"));
            context.Response.Headers.Append("Retry-After", retryAfter.ToString());

            await context.Response.WriteAsync($"Rate limit exceeded. Try again in {retryAfter} seconds.");
            return;
        }

        context.Response.Headers.Append("X-RateLimit-Limit", maxRequests.ToString());
        context.Response.Headers.Append("X-RateLimit-Remaining", (maxRequests - requestCount).ToString());
        context.Response.Headers.Append("X-RateLimit-Reset", currentTime.AddSeconds(windowSize).ToString("r"));

        await _next(context);
    }

    private static string GetClientId(HttpContext context)
    {
        var userIdClaim = context.User?.FindFirst("sub")?.Value ??
                         context.User?.FindFirst("userId")?.Value;

        if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var userId))
        {
            return $"user:{userId}";
        }

        var ipAddress = context.Connection.RemoteIpAddress?.ToString()
                     ?? context.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                     ?? context.Request.Headers["X-Real-IP"].FirstOrDefault()
                     ?? "unknown";

        return $"ip:{ipAddress}";
    }

    private static (int MaxRequests, int WindowSize) GetLimitsForClient(HttpContext context, RateLimitSettings settings)
    {
        var isAuthenticated = context.User?.Identity?.IsAuthenticated == true;

        if (isAuthenticated && settings.DifferentLimitsForAuth && settings.AuthenticatedUserMaxRequests.HasValue)
        {
            return (settings.AuthenticatedUserMaxRequests.Value, settings.WindowSizeInSeconds);
        }

        return (settings.MaxRequests, settings.WindowSizeInSeconds);
    }

    private static int CalculateRetryAfter(int windowSizeInSeconds)
    {
        return Math.Max(1, windowSizeInSeconds);
    }
}


