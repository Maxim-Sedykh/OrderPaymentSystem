using OrderPaymentSystem.Application.Interfaces.Services;

namespace OrderPaymentSystem.Api.Jobs
{
    public class AuthJobs
    {
        private readonly ITokenMaintenanceService _tokenMaintenance;

        public AuthJobs(ITokenMaintenanceService tokenMaintenance)
        {
            _tokenMaintenance = tokenMaintenance;
        }

        public async Task CleanupTokens(CancellationToken ct) =>
            await _tokenMaintenance.CleanupExpiredTokensAsync(ct);
    }
}
