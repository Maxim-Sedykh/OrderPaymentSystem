namespace OrderPaymentSystem.DAL.Settings;

/// <summary>
/// Настройки Redis
/// </summary>
public class RedisSettings
{
    /// <summary>
    /// Url до инстанса Redis
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Название инстанса
    /// </summary>
    public string? InstanceName { get; set; }
}
