using Microsoft.Extensions.Options;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Application.Interfaces.Services.Maintenance;
using OrderPaymentSystem.Application.Settings;
using OrderPaymentSystem.Application.Specifications;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Shared.Specifications;

namespace OrderPaymentSystem.Application.Services.Maintenance;

/// <summary>
/// Сервис для работы с Refresh-токеном в фоновых задачах
/// </summary>
internal class TokenMaintenanceService : ITokenMaintenanceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;
    private readonly int _refreshTokenValidityInDays;

    public TokenMaintenanceService(IUnitOfWork unitOfWork, TimeProvider timeProvider, IOptions<JwtSettings> jwtSettings)
    {
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
        _refreshTokenValidityInDays = jwtSettings.Value.RefreshTokenValidityInDays;
    }

    /// <inheritdoc/>
    public async Task CleanupExpiredTokensAsync(CancellationToken ct)
    {
        var now = _timeProvider.GetUtcNow().UtcDateTime;

        var spec = new BaseSpecification<UserToken>();
        var expiredTokensSpec = UserTokenSpecs.ExpiredBefore(spec, now.AddDays(-_refreshTokenValidityInDays));

        var expiredTokens = await _unitOfWork.UserTokens.GetListBySpecAsync(expiredTokensSpec, ct);

        _unitOfWork.UserTokens.RemoveRange(expiredTokens);

        await _unitOfWork.SaveChangesAsync(ct);
    }
}
