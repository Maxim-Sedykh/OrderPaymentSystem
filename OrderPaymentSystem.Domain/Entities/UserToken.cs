using OrderPaymentSystem.Domain.Interfaces.Entities;
using OrderPaymentSystem.Domain.Result;

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
    public static DataResult<UserToken> Create(Guid userId, string refreshToken, DateTime expireTime)
    {
        if (string.IsNullOrWhiteSpace(refreshToken)) return DataResult<UserToken>.Failure(5001, "Refresh token cannot be empty.");
        if (expireTime <= DateTime.UtcNow) return DataResult<UserToken>.Failure(5002, "Refresh token expiration must be in the future.");

        return DataResult<UserToken>.Success(new UserToken
        {
            Id = default,
            UserId = userId,
            RefreshToken = refreshToken,
            RefreshTokenExpireTime = expireTime
        });
    }

    /// <summary>
    /// Обновить токен
    /// </summary>
    /// <param name="newRefreshToken">Новый Refresh токен</param>
    /// <param name="newExpireTime">Новый срок истечения Refresh токена</param>
    /// <returns>Результат обновления токена</returns>
    public BaseResult UpdateToken(string newRefreshToken, DateTime newExpireTime)
    {
        if (string.IsNullOrWhiteSpace(newRefreshToken)) return BaseResult.Failure(5003, "Refresh token cannot be empty.");
        if (newExpireTime <= DateTime.UtcNow) return BaseResult.Failure(5004, "Refresh token expiration must be in the future.");

        RefreshToken = newRefreshToken;
        RefreshTokenExpireTime = newExpireTime;
        return BaseResult.Success();
    }

    /// <summary>
    /// Истек ли срок действия Refresh токена
    /// </summary>
    /// <returns></returns>
    public bool IsExpired() => RefreshTokenExpireTime <= DateTime.UtcNow;
}
