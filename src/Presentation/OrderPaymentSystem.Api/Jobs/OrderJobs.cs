using OrderPaymentSystem.Application.Interfaces.Services.Maintenance;

namespace OrderPaymentSystem.Api.Jobs;

/// <summary>
/// Фоновая задача для заказов.
/// </summary>
public class OrderJobs
{
    private readonly IOrderMaintenanceService _orderMaintenanceService;

    /// <summary>
    /// Конструктор задачи.
    /// </summary>
    /// <param name="orderMaintenanceService">Maintenance сервис для работы с заказами.</param>
    public OrderJobs(IOrderMaintenanceService orderMaintenanceService)
    {
        _orderMaintenanceService = orderMaintenanceService;
    }

    /// <summary>
    /// Отменить старые заказы.
    /// </summary>
    /// <param name="ct">Токен отмены операции</param>
    public async Task CancelExpiredOrders(CancellationToken ct) =>
        await _orderMaintenanceService.CancelExpiredPendingOrdersAsync(ct);
}
