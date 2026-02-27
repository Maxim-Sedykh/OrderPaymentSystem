namespace OrderPaymentSystem.Domain.Constants;

/// <summary>
/// Список дефолтных ролей
/// </summary>
public static class DefaultRoles
{
    /// <summary>
    /// Обычный пользователь
    /// </summary>
    public const string User = nameof(User);

    /// <summary>
    /// Модератор
    /// </summary>
    public const string Moderator = nameof(Moderator);

    /// <summary>
    /// Администратор
    /// </summary>
    public const string Admin = nameof(Admin);
}
