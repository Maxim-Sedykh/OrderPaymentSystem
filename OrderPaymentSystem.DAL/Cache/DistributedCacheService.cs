using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using OrderPaymentSystem.Domain.Interfaces.Cache;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OrderPaymentSystem.DAL.Cache;

/// <summary>
/// Реализация сервиса для работы с распределенным кэшем
/// </summary>
public sealed class DistributedCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<DistributedCacheService> _logger;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    /// <summary>
    /// Создает экземпляр <see cref="DistributedCacheService"/>
    /// </summary>
    public DistributedCacheService(
        IDistributedCache cache,
        ILogger<DistributedCacheService> logger)
    {
        _cache = cache;
        _logger = logger;

        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    /// <inheritdoc/>
    public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));

        var data = await _cache.GetAsync(key, cancellationToken);

        if (data is null || data.Length == 0)
        {
            return null;
        }

        var result = JsonSerializer.Deserialize<T>(data, _jsonSerializerOptions);

        if (result is null)
        {
            _logger.LogWarning("Failed to deserialize cached data for key: {CacheKey}", key);
        }

        return result;
    }

    /// <inheritdoc/>
    public async Task<T> GetOrCreateAsync<T>(string key,
        Func<CancellationToken, Task<T>> factory,
        DistributedCacheEntryOptions options = null,
        CancellationToken cancellationToken = default) where T : class
    {
        ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));
        ArgumentNullException.ThrowIfNull(factory, nameof(factory));

        var cachedValue = await GetAsync<T>(key, cancellationToken);

        if (cachedValue is not null)
        {
            return cachedValue;
        }

        var value = await factory(cancellationToken)
            ?? throw new InvalidOperationException($"Factory for key '{key}' returned null");

        await SetAsync(key, value, options, cancellationToken);

        return value;
    }

    /// <inheritdoc/>
    public async Task SetAsync<T>(string key,
        T value,
        DistributedCacheEntryOptions options = null,
        CancellationToken cancellationToken = default) where T : class
    {
        ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));
        ArgumentNullException.ThrowIfNull(value, nameof(value));

        var data = JsonSerializer.SerializeToUtf8Bytes(value, _jsonSerializerOptions);

        var cacheOptions = options ?? GetDefaultCacheOptions();

        await _cache.SetAsync(key, data, cacheOptions, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));

        await _cache.RemoveAsync(key, cancellationToken);
    }

    /// <summary>
    /// Получает опции кэширования по умолчанию
    /// </summary>
    private static DistributedCacheEntryOptions GetDefaultCacheOptions()
    {
        return new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
            SlidingExpiration = TimeSpan.FromMinutes(2)
        };
    }
}