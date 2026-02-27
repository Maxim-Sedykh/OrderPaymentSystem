using Microsoft.Extensions.Caching.Distributed;
using OrderPaymentSystem.Application.Interfaces.Cache;

namespace OrderPaymentSystem.Benchmarks.Mocks;

/// <summary>
/// Заглушка сервиса кэширования.
/// </summary>
public class NoCacheService : ICacheService
{
    /// <summary>
    /// Возвращаем null из кэша, как будто объекта с ключом key нет в кэше
    /// </summary>
    /// <typeparam name="T">Тип объекта</typeparam>
    /// <param name="key">Ключ</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>null</returns>
    public Task<T?> GetAsync<T>(string key, CancellationToken ct = default) where T : class? => Task.FromResult<T?>(null);

    /// <summary>
    /// Просто выполняем фабрику, как-будто кэша нет.
    /// </summary>
    /// <typeparam name="T">Тип объекта</typeparam>
    /// <param name="key">Ключ кэширования</param>
    /// <param name="factory">Делегат для получения объекта</param>
    /// <param name="options">Опции кэширования</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns></returns>
    public Task<T?> GetOrCreateAsync<T>(string key,
        Func<CancellationToken,
        Task<T?>> factory,
        DistributedCacheEntryOptions? options = null,
        CancellationToken ct = default) where T : class? => factory(ct);

    /// <summary>
    /// Метод возвращает завершённую задачу, имитирование удаления кэша по ключу
    /// </summary>
    /// <param name="key">Ключ</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Завершённая задача</returns>
    public Task RemoveAsync(string key, CancellationToken ct = default) => Task.CompletedTask;

    /// <summary>
    /// Метод возвращает завершённую задачу, имитирование создания объекта по ключу в кэше
    /// </summary>
    /// <typeparam name="T">Тип объекта</typeparam>
    /// <param name="key">Ключ кэширования</param>
    /// <param name="obj">Объект который якобы кладём в кэш</param>
    /// <param name="opt">Опции кэширования</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Завершённая задача</returns>
    public Task SetAsync<T>(string key,
        T obj,
        DistributedCacheEntryOptions? opt = null,
        CancellationToken ct = default) where T : class? => Task.CompletedTask;
}
