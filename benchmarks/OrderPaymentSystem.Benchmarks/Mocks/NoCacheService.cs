using Microsoft.Extensions.Caching.Distributed;
using OrderPaymentSystem.Application.Interfaces.Cache;

namespace OrderPaymentSystem.Benchmarks.Mocks;

public class NoCacheService : ICacheService
{
    public Task<T> GetAsync<T>(string key, CancellationToken ct = default) where T : class => Task.FromResult<T>(null);
    public Task<T> GetOrCreateAsync<T>(string key, Func<CancellationToken, Task<T>> factory, DistributedCacheEntryOptions options = null, CancellationToken ct = default) where T : class => factory(ct);
    public Task RemoveAsync(string key, CancellationToken ct = default) => Task.CompletedTask;
    public Task SetAsync<T>(string key, T obj, DistributedCacheEntryOptions opt = null, CancellationToken ct = default) where T : class => Task.CompletedTask;
}
