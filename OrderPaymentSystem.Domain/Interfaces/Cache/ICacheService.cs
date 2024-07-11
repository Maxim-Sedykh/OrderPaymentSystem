using Microsoft.Extensions.Caching.Distributed;

namespace OrderPaymentSystem.Domain.Interfaces.Cache
{
    /// <summary>
    /// Сервис для работы с кэшем
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Получить объект из кэша по ключу
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T> GetObjectAsync<T>(string key) where T: class;

        /// <summary>
        /// Получить объект из кэша по ключу
        /// Если объекта нет в кэше - то получаем его из базы данных и добавляем его в кэш
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        Task<T> GetObjectAsync<T>(string key, Func<Task<T>> factory) where T : class;

        /// <summary>
        /// Добавление объекта в кэш
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        Task SetObjectAsync<T>(string key, T obj, DistributedCacheEntryOptions options = null) where T: class;
    }
}
