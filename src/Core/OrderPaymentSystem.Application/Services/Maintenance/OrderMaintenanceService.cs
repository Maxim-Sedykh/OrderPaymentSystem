using Microsoft.Extensions.Logging;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Application.Interfaces.Services.Maintenance;
using OrderPaymentSystem.Application.Specifications;
using OrderPaymentSystem.Domain.Enum;

namespace OrderPaymentSystem.Application.Services.Maintenance;

/// <summary>
/// Сервис для работы с заказами в фоновых задачах
/// </summary>
internal class OrderMaintenanceService : IOrderMaintenanceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<OrderMaintenanceService> _logger;
    private readonly TimeProvider _timeProvider;

    public OrderMaintenanceService(IUnitOfWork unitOfWork, ILogger<OrderMaintenanceService> logger, TimeProvider timeProvider)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _timeProvider = timeProvider;
    }

    /// <inheritdoc/>
    public async Task CancelExpiredPendingOrdersAsync(CancellationToken ct = default)
    {
        try
        {
            var expirationThreshold = _timeProvider.GetUtcNow().AddHours(-24).UtcDateTime;

            var expiredPendingOrders = await _unitOfWork.Orders.GetListBySpecAsync(
                OrderSpecs.ByStatus(OrderStatus.Pending).CreatedBefore(expirationThreshold),
                ct);

            if (expiredPendingOrders.Count == 0)
            {
                _logger.LogInformation("No expired pending orders found to cancel.");
                return;
            }

            _logger.LogInformation("Found {Count} expired pending orders to cancel.", expiredPendingOrders.Count);

            foreach (var order in expiredPendingOrders)
            {
                _logger.LogInformation("Cancelling expired pending order: {OrderId} (Created: {CreatedAt})", order.Id, order.CreatedAt);

                order.UpdateStatus(OrderStatus.Cancelled);
            }

            await _unitOfWork.SaveChangesAsync(ct);
            _logger.LogInformation("Successfully cancelled {Count} expired pending orders.", expiredPendingOrders.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while cancelling pending orders");
        }
    }
}
