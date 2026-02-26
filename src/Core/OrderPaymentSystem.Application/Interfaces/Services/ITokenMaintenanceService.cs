using System;
using System.Collections.Generic;
using System.Text;

namespace OrderPaymentSystem.Application.Interfaces.Services
{
    public interface ITokenMaintenanceService
    {
        Task CleanupExpiredTokensAsync(CancellationToken ct);
    }
}
