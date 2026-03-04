namespace OrderPaymentSystem.Application.Settings;

/// <summary>
/// Креды пользователя с ролью Админ
/// </summary>
public sealed class AdminSettings
{
    /// <summary>
    /// Логин
    /// </summary>
    public string? Login { get; set; }

    /// <summary>
    /// Пароль
    /// </summary>
    public string? Password { get; set; }
}
