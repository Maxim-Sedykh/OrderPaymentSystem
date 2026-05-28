namespace OrderPaymentSystem.Application.Interfaces.RateLimit;

/// <summary>
/// Интерфейс для хранения данных rate limiting в Redis
/// </summary>
public interface IRateLimitStore
{
    /// <summary>
    /// Проверяет и добавляет запрос в окно
    /// </summary>
    /// <param name="key">Ключ клиента (user:id или ip:address)</param>
    /// <param name="windowSizeInSeconds">Размер окна в секундах</param>
    /// <param name="maxRequests">Максимальное количество запросов</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Количество сделанных запросов и можно ли сделать ещё один</returns>
    Task<(int RequestCount, bool Allowed)> CheckAndAddRequestAsync(
        string key,
        int windowSizeInSeconds,
        int maxRequests,
        CancellationToken ct = default);

    /// <summary>
    /// Сбрасывает счётчик для клиента
    /// </summary>
    /// <param name="key">Ключ клиента</param>
    /// <param name="ct">Токен отмены</param>
    Task ResetAsync(string key, CancellationToken ct = default);
}
