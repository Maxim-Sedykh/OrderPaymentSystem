using Microsoft.Extensions.Caching.Distributed;

namespace OrderPaymentSystem.Domain.Interfaces.Cache;

/// <summary>
/// Интерфейс сервиса для работы с кэшем
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Получить объект из кэша по ключу
    /// </summary>
    /// <typeparam name="T">Тип объекта который получаем из кэша</typeparam>
    /// <param name="key">Ключ кэширования</param>
    /// <returns>Объект</returns>
    Task<T> GetObjectAsync<T>(string key) where T: class;

    /// <summary>
    /// Получить объект из кэша по ключу
    /// Если объекта нет в кэше - то получаем его из базы данных и добавляем его в кэш
    /// </summary>
    /// <typeparam name="T">Тип объекта который получаем из кэша</typeparam>
    /// <param name="key">Ключ кэширования</param>
    /// <param name="factory">Делегат, для получения объекта из другого источника, если его нет в кэше</param>
    /// <returns>Объект</returns>
    Task<T> GetObjectAsync<T>(string key, Func<Task<T>> factory) where T : class;

    /// <summary>
    /// Добавление объекта в кэш
    /// </summary>
    /// <typeparam name="T">Тип объекта который получаем из кэша</typeparam>
    /// <param name="key">Ключ кэширования</param>
    /// <param name="obj">Объект, который будет кэшироваться</param>
    /// <param name="options">Опции кэширования</param>
    Task SetObjectAsync<T>(string key, T obj, DistributedCacheEntryOptions options = null) where T: class;
}
