using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Domain.Interfaces.Entities;
using OrderPaymentSystem.Shared.Exceptions;

namespace OrderPaymentSystem.Domain.Entities;

/// <summary>
/// JWT-токен пользователя
/// </summary>
public class UserToken : IEntityId<long>
{
    /// <summary>
    /// Id токена
    /// </summary>
    public long Id { get; protected set; }

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
    public static UserToken Create(Guid userId, string refreshToken, DateTime expireTime) //TODO проверки тоже вынести
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new BusinessException(DomainErrors.Token.RefreshEmpty());

        if (expireTime <= DateTime.UtcNow)
            throw new BusinessException(DomainErrors.Token.RefreshFuture());

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
        if (string.IsNullOrWhiteSpace(newRefreshToken))
            throw new BusinessException(DomainErrors.Token.RefreshEmpty());

        if (newExpireTime <= DateTime.UtcNow)
            throw new BusinessException(DomainErrors.Token.RefreshFuture());

        RefreshToken = newRefreshToken;
        RefreshTokenExpireTime = newExpireTime;
    }

    /// <summary>
    /// Истек ли срок действия Refresh токена
    /// </summary>
    /// <returns></returns>
    public bool IsExpired() => RefreshTokenExpireTime <= DateTime.UtcNow;
}
