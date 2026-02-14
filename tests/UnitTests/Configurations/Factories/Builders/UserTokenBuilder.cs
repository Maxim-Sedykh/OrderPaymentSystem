using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.UnitTests.Configurations.Factories.Builders;


public class UserTokenBuilder
{
    private readonly string _token = "refresh_token";
    private readonly DateTime _expire = DateTime.UtcNow.AddDays(7);

    public UserToken Build(Guid userId) => UserToken.Create(userId, _token, _expire, DateTime.UtcNow);
}
