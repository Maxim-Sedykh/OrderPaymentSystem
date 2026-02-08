using OrderPaymentSystem.Domain.Abstract;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Entities;
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
    public string RefreshToken { get; protected set; }

    /// <summary>
    /// Время истечения действия Refresh токена
    /// </summary>
    public DateTime RefreshTokenExpireTime { get; protected set; }

    /// <summary>
    /// Id пользователя
    /// </summary>
    public Guid UserId { get; protected set; }

    /// <summary>
    /// Пользователь
    /// </summary>
    public User User { get; protected set; }

    protected UserToken() { }

    /// <summary>
    /// Создать токен
    /// </summary>
    /// <param name="userId">Id пользователя</param>
    /// <param name="refreshToken">Refresh токен</param>
    /// <param name="expireTime">Время истечения жизни токена</param>
    /// <returns>Результат создания</returns>
    public static UserToken Create(Guid userId, string refreshToken, DateTime expireTime)
    {
        Validate(refreshToken, expireTime);

        return new UserToken
        {
            Id = default,
            UserId = userId,
            RefreshToken = refreshToken,
            RefreshTokenExpireTime = expireTime
        };
    }

    /// <summary>
    /// Обновить токен
    /// </summary>
    /// <param name="newRefreshToken">Новый Refresh токен</param>
    /// <param name="newExpireTime">Новый срок истечения Refresh токена</param>
    /// <returns>Результат обновления токена</returns>
    public void UpdateRefreshTokenData(string newRefreshToken, DateTime newExpireTime)
    {
        Validate(newRefreshToken, newExpireTime);

        RefreshToken = newRefreshToken;
        RefreshTokenExpireTime = newExpireTime;
    }

    /// <summary>
    /// Истек ли срок действия Refresh токена
    /// </summary>
    /// <returns></returns>
    public bool IsExpired() => RefreshTokenExpireTime <= DateTime.UtcNow;

    private static void Validate(string token, DateTime expire)
    {
        if (string.IsNullOrWhiteSpace(token)) throw new BusinessException(DomainErrors.Validation.Required(nameof(token)));
        if (expire <= DateTime.UtcNow) throw new BusinessException(DomainErrors.Token.RefreshFuture());
    }
}
