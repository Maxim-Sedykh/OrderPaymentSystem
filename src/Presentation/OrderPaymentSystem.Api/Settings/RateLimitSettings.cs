namespace OrderPaymentSystem.Api.Settings;

/// <summary>
/// Настройки для Rate Limiting middleware
/// </summary>
public sealed class RateLimitSettings
{
    /// <summary>
    /// Включен ли Rate Limiting
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Максимальное количество запросов в окне
    /// </summary>
    public int MaxRequests { get; set; } = 100;

    /// <summary>
    /// Размер временного окна в секундах
    /// </summary>
    public int WindowSizeInSeconds { get; set; } = 60;

    /// <summary>
    /// Применять ли разные лимиты для аутентифицированных и неаутентифицированных пользователей
    /// </summary>
    public bool DifferentLimitsForAuth { get; set; } = false;

    /// <summary>
    /// Лимит запросов для аутентифицированных пользователей
    /// </summary>
    public int? AuthenticatedUserMaxRequests { get; set; }

    /// <summary>
    /// Пути, исключенные из Rate Limiting
    /// </summary>
    public List<string> ExcludedPaths { get; set; } =
    [
        "/health",
        "/health/ready",
        "/health/live",
        "/metrics",
        "/swagger",
        "/api/v1/auth/login"
    ];
}
