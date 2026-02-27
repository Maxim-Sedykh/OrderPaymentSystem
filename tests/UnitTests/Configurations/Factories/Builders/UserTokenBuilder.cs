using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.UnitTests.Configurations.Factories.Builders;

/// <summary>
/// Билдер для построения мокового токена пользователя
/// </summary>
public class UserTokenBuilder
{
    private readonly string _token = "refresh_token";
    private readonly DateTime _expire = DateTime.UtcNow.AddDays(7);

    /// <summary>
    /// Построить, создать объект.
    /// </summary>
    /// <returns>Созданный токен</returns>
    public UserToken Build(Guid userId) => UserToken.Create(userId, _token, _expire, DateTime.UtcNow);
}
