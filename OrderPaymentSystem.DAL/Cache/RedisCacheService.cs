using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using OrderPaymentSystem.Domain.Interfaces.Cache;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.DAL.Cache
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDistributedCache _distributedCache;

        public RedisCacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<T> GetAsync<T>(string key) where T : class
        {
            string cachedValue = await _distributedCache.GetStringAsync(key);

            if (cachedValue is null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<T>(cachedValue);
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> factory) where T : class
        {
            T cachedValue = await GetAsync<T>(key);

            if (cachedValue is not null)
            {
                return cachedValue;
            }

            cachedValue = await factory();

            await SetValueAsync(key, cachedValue);

            return cachedValue;
        }

        public async Task SetValueAsync<T>(string key, T value) where T : class
        {
            string cacheValue = JsonConvert.SerializeObject(value);

            await _distributedCache.SetStringAsync(key, cacheValue, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
            });
        }
    }
}
