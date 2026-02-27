using OrderPaymentSystem.Domain.Abstract;
using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Shared.Exceptions;

namespace OrderPaymentSystem.Domain.Entities;

/// <summary>
/// JWT-токен пользователя
/// </summary>
public class UserToken : BaseEntity<long>
{
    /// <summary>
    /// Refresh токен
    /// </summary>
    public string RefreshToken { get; private set; } = string.Empty;

    /// <summary>
    /// Время истечения действия Refresh токена
    /// </summary>
    public DateTime RefreshTokenExpireTime { get; private set; }

    /// <summary>
    /// Id пользователя
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Пользователь
    /// </summary>
    public User? User { get; private set; }

    private UserToken() { }

    private UserToken(Guid userId, string refreshToken, DateTime expireTime)
    {
        UserId = userId;
        RefreshToken = refreshToken;
        RefreshTokenExpireTime = expireTime;
    }

    /// <summary>
    /// Создать токен
    /// </summary>
    /// <param name="userId">Id пользователя</param>
    /// <param name="refreshToken">Refresh токен</param>
    /// <param name="expireTime">Время истечения жизни токена</param>
    /// <param name="nowUtc">Текущая дата и время</param>
    /// <returns>Результат создания</returns>
    public static UserToken Create(Guid userId, string refreshToken, DateTime expireTime, DateTime nowUtc)
    {
        Validate(refreshToken, expireTime, nowUtc);

        return new UserToken(userId, refreshToken, expireTime);
    }

    /// <summary>
    /// Обновить токен
    /// </summary>
    /// <param name="newRefreshToken">Новый Refresh токен</param>
    /// <param name="newExpireTime">Новый срок истечения Refresh токена</param>
    /// <param name="nowUtc">Текущая дата и время</param>
    /// <returns>Результат обновления токена</returns>
    public void UpdateRefreshTokenData(string newRefreshToken, DateTime newExpireTime, DateTime nowUtc)
    {
        Validate(newRefreshToken, newExpireTime, nowUtc);

        RefreshToken = newRefreshToken;
        RefreshTokenExpireTime = newExpireTime;
    }

    /// <summary>
    /// Истек ли срок действия Refresh токена
    /// </summary>
    /// <returns></returns>
    public bool IsExpired(DateTime date) => RefreshTokenExpireTime <= date;

    /// <summary>
    /// Валидировать входные данные
    /// </summary>
    /// <param name="token">Refresh-токен</param>
    /// <param name="expire">Время истечения срока годности</param>
    /// <param name="nowUtc">Текущая дата и время</param>
    /// <exception cref="BusinessException"></exception>
    private static void Validate(string token, DateTime expire, DateTime nowUtc)
    {
        if (string.IsNullOrWhiteSpace(token)) throw new BusinessException(DomainErrors.Validation.Required(nameof(token)));
        if (expire <= nowUtc) throw new BusinessException(DomainErrors.Token.RefreshFuture());
    }
}
