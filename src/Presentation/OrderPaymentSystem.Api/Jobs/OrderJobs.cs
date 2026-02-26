using OrderPaymentSystem.Application.Interfaces.Services;

namespace OrderPaymentSystem.Api.Jobs
{
    public class OrderJobs
    {
        private readonly IOrderMaintenanceService _orderMaintenanceService;

        public OrderJobs(IOrderMaintenanceService orderMaintenanceService)
        {
            _orderMaintenanceService = orderMaintenanceService;
        }

        public async Task CancelExpiredOrders(CancellationToken ct) =>
            await _orderMaintenanceService.CancelExpiredPendingOrdersAsync(ct);
    }
}
