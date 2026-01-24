using Microsoft.Extensions.Caching.Distributed;

namespace OrderPaymentSystem.Application.Interfaces.Cache;

/// <summary>
/// Интерфейс сервиса для работы с распределенным кэшем
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Получить объект из кэша по ключу
    /// </summary>
    /// <typeparam name="T">Тип объекта который получаем из кэша</typeparam>
    /// <param name="key">Ключ кэширования</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Десериализованный объект или null, если не найден</returns>
    Task<T> GetAsync<T>(string key, CancellationToken ct = default) where T: class;

    /// <summary>
    /// Получает объект из кэша или получает его из БД, если отсутствует
    /// </summary>
    /// <typeparam name="T">Тип объекта который получаем из кэша</typeparam>
    /// <param name="key">Ключ кэширования</param>
    /// <param name="factory">Фабрика для создания объекта при его отсутствии в кэше</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Кэшированный или созданный объект</returns>
    Task<T> GetOrCreateAsync<T>(string key,
        Func<CancellationToken, Task<T>> factory,
        DistributedCacheEntryOptions options = null,
        CancellationToken ct = default) where T : class;

    /// <summary>
    /// Добавление объекта в кэш
    /// </summary>
    /// <typeparam name="T">Тип объекта который получаем из кэша</typeparam>
    /// <param name="key">Ключ кэширования</param>
    /// <param name="obj">Объект, который будет кэшироваться</param>
    /// <param name="options">Опции кэширования</param>
    /// <param name="ct">Токен отмены операции</param>
    Task SetAsync<T>(string key, T obj, DistributedCacheEntryOptions options = null, CancellationToken ct = default) where T : class;

    /// <summary>
    /// Убрать объект из кэша по ключу
    /// </summary>
    /// <param name="key">Ключ</param>
    /// <param name="ct">Токен отмены</param>
    Task RemoveAsync(string key, CancellationToken ct = default);
}
