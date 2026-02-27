namespace OrderPaymentSystem.Application.Settings;

/// <summary>
/// Представляет настройки конфигурации для JSON Web Token (JWT).
/// Используется для параметров выпуска и проверки токенов.
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// Издатель (Issuer) JWT токена. Указывает, кто выпустил токен.
    /// Например: "yourdomain.com" или "MyAuthServer".
    /// </summary>
    public string? Issuer { get; set; }

    /// <summary>
    /// Аудитория (Audience) JWT токена. Указывает, для кого предназначен токен.
    /// Например: "your_api_resource" или "yourwebapp.com".
    /// </summary>
    public string? Audience { get; set; }

    /// <summary>
    /// Авторитет (Authority) для проверки токенов.
    /// Используется для OpenID Connect (OIDC) и содержит URL сервера авторизации.
    /// Например: "https://login.microsoftonline.com/your-tenant-id/v2.0".
    /// </summary>
    public string? Authority { get; set; }

    /// <summary>
    /// Секретный ключ (Symmetric Key) для подписи и проверки JWT токенов.
    /// Должен быть достаточно длинным и храниться в безопасности.
    /// </summary>
    public string? JwtKey { get; set; }

    /// <summary>
    /// Строковое представление времени жизни токена (например, "00:05:00" для 5 минут).
    /// Используется для настройки срока действия токена доступа.
    /// </summary>
    public string? Lifetime { get; set; }

    /// <summary>
    /// Срок действия Refresh Token в днях.
    /// Определяет, как долго Refresh Token будет действителен для получения новых Access Token.
    /// </summary>
    public int RefreshTokenValidityInDays { get; set; }

    /// <summary>
    /// Срок действия Access Token в минутах.
    /// Определяет, как долго Access Token будет действителен до истечения срока.
    /// </summary>
    public int AccessTokenValidityInMinutes { get; set; }
}