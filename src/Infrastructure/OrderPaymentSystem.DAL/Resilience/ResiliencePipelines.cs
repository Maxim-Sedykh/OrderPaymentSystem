using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using OrderPaymentSystem.Application.Settings;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using StackExchange.Redis;

namespace OrderPaymentSystem.DAL.Resilience;

/// <summary>
/// Фабрики resilience-пайплайнов для Cache, Database и HTTP.
/// </summary>
public static class ResiliencePipelines
{
    /// <summary>
    /// Настроить пайплайн для Redis: Retry на transient-ошибки + Timeout.
    /// </summary>
    public static void ConfigureCache(ResiliencePipelineBuilder builder, PipelineOptions options, ILogger logger)
    {
        builder
            .AddRetry(new RetryStrategyOptions
            {
                ShouldHandle = new PredicateBuilder()
                    .Handle<RedisConnectionException>()
                    .Handle<RedisTimeoutException>()
                    .Handle<RedisException>(),
                MaxRetryAttempts = options.RetryCount,
                Delay = TimeSpan.FromSeconds(options.RetryDelaySeconds),
                BackoffType = DelayBackoffType.Exponential,
                OnRetry = args =>
                {
                    logger.LogWarning(args.Outcome.Exception,
                        "Redis retry attempt {Attempt} after {Delay}ms",
                        args.AttemptNumber, args.RetryDelay.TotalMilliseconds);
                    return default;
                }
            })
            .AddTimeout(TimeSpan.FromSeconds(options.TimeoutSeconds));
    }

    /// <summary>
    /// Настроить пайплайн для БД: Retry на transient-ошибки + Circuit Breaker.
    /// </summary>
    public static void ConfigureDatabase(ResiliencePipelineBuilder builder, PipelineOptions options, ILogger logger)
    {
        builder
            .AddRetry(new RetryStrategyOptions
            {
                ShouldHandle = new PredicateBuilder()
                    .Handle<DbUpdateException>()
                    .Handle<NpgsqlException>()
                    .Handle<TimeoutException>(),
                MaxRetryAttempts = options.RetryCount,
                Delay = TimeSpan.FromSeconds(options.RetryDelaySeconds),
                BackoffType = DelayBackoffType.Exponential,
                OnRetry = args =>
                {
                    logger.LogWarning(args.Outcome.Exception,
                        "Database retry attempt {Attempt} after {Delay}ms",
                        args.AttemptNumber, args.RetryDelay.TotalMilliseconds);
                    return default;
                }
            })
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions
            {
                FailureRatio = options.FailureRatio,
                SamplingDuration = TimeSpan.FromSeconds(options.SamplingDurationSeconds),
                MinimumThroughput = options.MinimumThroughput,
                BreakDuration = TimeSpan.FromSeconds(options.BreakDurationSeconds),
                OnOpened = args =>
                {
                    logger.LogError("Database circuit breaker opened for {Duration}ms",
                        args.BreakDuration.TotalMilliseconds);
                    return default;
                },
                OnClosed = _ => { logger.LogInformation("Database circuit breaker closed"); return default; },
                OnHalfOpened = _ => { logger.LogInformation("Database circuit breaker half-opened"); return default; }
            });
    }

    /// <summary>
    /// Настроить пайплайн для HTTP: Retry + Circuit Breaker + Timeout.
    /// </summary>
    public static void ConfigureHttp(ResiliencePipelineBuilder<HttpResponseMessage> builder, PipelineOptions options, ILogger logger)
    {
        builder
            .AddRetry(new RetryStrategyOptions<HttpResponseMessage>
            {
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<HttpRequestException>()
                    .HandleResult(r => !r.IsSuccessStatusCode),
                MaxRetryAttempts = options.RetryCount,
                Delay = TimeSpan.FromSeconds(options.RetryDelaySeconds),
                BackoffType = DelayBackoffType.Exponential,
                OnRetry = args =>
                {
                    logger.LogWarning(args.Outcome.Exception,
                        "HTTP retry attempt {Attempt} after {Delay}ms",
                        args.AttemptNumber, args.RetryDelay.TotalMilliseconds);
                    return default;
                }
            })
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions<HttpResponseMessage>
            {
                FailureRatio = options.FailureRatio,
                SamplingDuration = TimeSpan.FromSeconds(options.SamplingDurationSeconds),
                MinimumThroughput = options.MinimumThroughput,
                BreakDuration = TimeSpan.FromSeconds(options.BreakDurationSeconds),
                OnOpened = args =>
                {
                    logger.LogError("HTTP circuit breaker opened for {Duration}ms",
                        args.BreakDuration.TotalMilliseconds);
                    return default;
                },
                OnClosed = _ => { logger.LogInformation("HTTP circuit breaker closed"); return default; }
            })
            .AddTimeout(TimeSpan.FromSeconds(options.TimeoutSeconds));
    }
}
