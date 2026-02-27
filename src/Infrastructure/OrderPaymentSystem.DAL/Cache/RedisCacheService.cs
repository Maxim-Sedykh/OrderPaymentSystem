using MessagePack;
using MessagePack.Resolvers;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using OrderPaymentSystem.Application.Interfaces.Cache;

namespace OrderPaymentSystem.DAL.Cache;

/// <summary>
/// Реализация сервиса для работы с распределенным кэшем. Использую MessagePack,
/// чтобы сериализация строки с кэшем в объекты происходило быстрее.
/// </summary>
public sealed class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly MessagePackSerializerOptions _options;

    /// <summary>
    /// Создает экземпляр <see cref="RedisCacheService"/>
    /// </summary>
    public RedisCacheService(
        IDistributedCache cache,
        ILogger<RedisCacheService> logger)
    {
        _cache = cache;
        _logger = logger;

        var resolver = CompositeResolver.Create(
            ContractlessStandardResolver.Instance,
            StandardResolver.Instance
        );

        _options = MessagePackSerializerOptions.Standard
            .WithResolver(resolver);

        MessagePackSerializer.DefaultOptions = _options;
    }

    /// <inheritdoc/>
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class?
    {
        ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));

        var data = await _cache.GetAsync(key, cancellationToken);

        if (data is null || data.Length == 0)
        {
            return null;
        }

        try
        {
            return MessagePackSerializer.Deserialize<T>(data, _options, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to deserialize cached data for key: {CacheKey}", key);
            await _cache.RemoveAsync(key, cancellationToken);
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<T?> GetOrCreateAsync<T>(string key,
        Func<CancellationToken, Task<T?>> factory,
        DistributedCacheEntryOptions? options = null,
        CancellationToken cancellationToken = default) where T : class?
    {
        ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));
        ArgumentNullException.ThrowIfNull(factory, nameof(factory));

        var cachedValue = await GetAsync<T>(key, cancellationToken);

        if (cachedValue is not null)
        {
            return cachedValue;
        }

        var value = await factory(cancellationToken);
        if (value is null)
        {
            return null;
        }

        await SetAsync(key, value, options, cancellationToken);

        return value;
    }

    /// <inheritdoc/>
    public async Task SetAsync<T>(string key,
        T value,
        DistributedCacheEntryOptions? options = null,
        CancellationToken cancellationToken = default) where T : class?
    {
        ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));
        ArgumentNullException.ThrowIfNull(value, nameof(value));

        var data = MessagePackSerializer.Serialize(value, cancellationToken: cancellationToken);

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
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15),
            SlidingExpiration = TimeSpan.FromMinutes(5)
        };
    }
}