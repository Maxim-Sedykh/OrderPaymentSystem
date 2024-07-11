using Microsoft.Extensions.Caching.Distributed;
using OrderPaymentSystem.Domain.Interfaces.Cache;
using System.Text.Json;

namespace OrderPaymentSystem.DAL.Cache
{
    /// <inheritdoc/>
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;

        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        /// <inheritdoc/>
        public async Task<T> GetObjectAsync<T>(string key) where T : class
        {
            var data = await _cache.GetAsync(key);
            return data != null ? JsonSerializer.Deserialize<T>(data) : default;
        }

        /// <inheritdoc/>
        public async Task<T> GetObjectAsync<T>(string key, Func<Task<T>> factory) where T : class
        {
            T cachedValue = await GetObjectAsync<T>(key);

            if (cachedValue != null)
            {
                return cachedValue;
            }

            cachedValue = await factory();

            await SetObjectAsync(key, cachedValue);

            return cachedValue;
        }

        /// <inheritdoc/>
        public async Task SetObjectAsync<T>(string key, T obj, DistributedCacheEntryOptions options = null) where T : class
        {
            var data = JsonSerializer.SerializeToUtf8Bytes(obj);
            if (data != null && data.Length > 0)
            {
                await _cache.SetAsync(key, data, options ?? new DistributedCacheEntryOptions());
            }
        }
    }
}
