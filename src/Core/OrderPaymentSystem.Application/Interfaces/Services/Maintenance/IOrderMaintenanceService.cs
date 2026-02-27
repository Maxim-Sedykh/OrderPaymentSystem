namespace OrderPaymentSystem.Application.Interfaces.Services.Maintenance;

/// <summary>
/// Интерфейс сервиса для работы с заказами в фоновых задачах
/// </summary>
public interface IOrderMaintenanceService
{
    /// <summary>
    /// Отменить старые заказы
    /// </summary>
    /// <param name="ct">Токен отмены операции</param>
    Task CancelExpiredPendingOrdersAsync(CancellationToken ct = default);
}
