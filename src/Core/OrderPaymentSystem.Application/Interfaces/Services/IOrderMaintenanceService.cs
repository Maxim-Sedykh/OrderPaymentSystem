using System;
using System.Collections.Generic;
using System.Text;

namespace OrderPaymentSystem.Application.Interfaces.Services
{
    public interface IOrderMaintenanceService
    {
        Task CancelExpiredPendingOrdersAsync(CancellationToken ct = default);
    }
}
