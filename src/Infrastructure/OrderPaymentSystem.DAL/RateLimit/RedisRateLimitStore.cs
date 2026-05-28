using Microsoft.Extensions.Logging;
using OrderPaymentSystem.Application.Interfaces.RateLimit;
using StackExchange.Redis;
using System.Diagnostics;

namespace OrderPaymentSystem.DAL.RateLimit;

/// <summary>
/// Реализация хранилища rate limiting на основе Redis Sorted Sets
/// Использует sliding window algorithm для точного ограничения запросов
/// </summary>
public sealed class RedisRateLimitStore(
    IConnectionMultiplexer connectionMultiplexer,
    ILogger<RedisRateLimitStore> logger) : IRateLimitStore
{
    private const string RateLimitKeyPrefix = "ratelimit:";

    private readonly IConnectionMultiplexer _connectionMultiplexer = connectionMultiplexer;
    private readonly ILogger<RedisRateLimitStore> _logger = logger;

    /// <inheritdoc/>
    public async Task<(int RequestCount, bool Allowed)> CheckAndAddRequestAsync(
        string key,
        int windowSizeInSeconds,
        int maxRequests,
        CancellationToken ct = default)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var redisKey = GetRedisKey(key);
        var currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var windowStart = currentTime - windowSizeInSeconds;

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var transaction = db.CreateTransaction();

            var removeTask = transaction.SortedSetRemoveRangeByScoreAsync(
                redisKey,
                double.NegativeInfinity,
                windowStart);

            var countBeforeAddTask = transaction.SortedSetLengthAsync(redisKey);

            var addTask = transaction.SortedSetAddAsync(
                redisKey,
                currentTime.ToString(),
                currentTime);

            var countAfterAddTask = transaction.SortedSetLengthAsync(redisKey);

            var expireTask = transaction.KeyExpireAsync(
                redisKey,
                TimeSpan.FromSeconds(windowSizeInSeconds + 1));

            var executed = await transaction.ExecuteAsync();

            if (!executed)
            {
                _logger.LogWarning("Redis transaction failed for key {Key}", key);
                return (0, true);
            }

            await Task.WhenAll(removeTask, countBeforeAddTask, addTask, countAfterAddTask, expireTask);

            var currentCount = (int)countAfterAddTask.Result;
            var allowed = currentCount <= maxRequests;

            _logger.LogDebug(
                "RateLimit check: Key={Key}, Count={Count}, Max={Max}, Allowed={Allowed}, Latency={Latency}ms",
                key,
                currentCount,
                maxRequests,
                allowed,
                stopwatch.ElapsedMilliseconds);

            return (currentCount, allowed);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Redis rate limit check failed for key {Key}", key);

            return (0, true);
        }
    }

    /// <inheritdoc/>
    public async Task ResetAsync(string key, CancellationToken ct = default)
    {
        try
        {
            var db = _connectionMultiplexer.GetDatabase();
            var redisKey = GetRedisKey(key);
            await db.KeyDeleteAsync(redisKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reset rate limit for key {Key}", key);
        }
    }

    private static string GetRedisKey(string key) => $"{RateLimitKeyPrefix}{key}";
}
