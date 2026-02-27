namespace OrderPaymentSystem.Application.Interfaces.Services.Maintenance;

/// <summary>
/// Интерфейс сервиса для работы с Refresh-токенами в фоновых задачах
/// </summary>
public interface ITokenMaintenanceService
{
    /// <summary>
    /// Удалить старые Refresh-токены
    /// </summary>
    /// <param name="ct">Токен отмены операции</param>
    Task CleanupExpiredTokensAsync(CancellationToken ct);
}
