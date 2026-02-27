using OrderPaymentSystem.Application.Interfaces.Services.Maintenance;

namespace OrderPaymentSystem.Api.Jobs;

/// <summary>
/// Фоновая задача для данных связанных с авторизацией и аутентификацией пользователей.
/// </summary>
public class AuthJobs
{
    private readonly ITokenMaintenanceService _tokenMaintenance;

    /// <summary>
    /// Конструктор фоновой задачи.
    /// </summary>
    /// <param name="tokenMaintenance">Maintenance сервис для работы с токенами.</param>
    public AuthJobs(ITokenMaintenanceService tokenMaintenance)
    {
        _tokenMaintenance = tokenMaintenance;
    }

    /// <summary>
    /// Очистить из базы старые Refresh-токены.
    /// </summary>
    /// <param name="ct">Токен отмены операции</param>
    public async Task CleanupTokens(CancellationToken ct) =>
        await _tokenMaintenance.CleanupExpiredTokensAsync(ct);
}
