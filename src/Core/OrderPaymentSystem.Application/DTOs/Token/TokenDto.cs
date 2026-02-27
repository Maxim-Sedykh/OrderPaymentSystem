namespace OrderPaymentSystem.Application.DTOs.Token;

/// <summary>
/// Модель данных представления токена
/// </summary>
public class TokenDto
{
    /// <summary>
    /// Access-токен, передаётся клиентом
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Refresh-токен, хранится на сервере
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
}
