using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using Moq;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Application.Services.Auth;
using OrderPaymentSystem.Application.Settings;

namespace OrderPaymentSystem.UnitTests.Configurations.Fixtures;

internal class TokenFixture
{
    public UserTokenService Service { get; }
    public FakeTimeProvider TimeProvider { get; } = new();

    public TokenFixture()
    {
        var settings = Options.Create(new JwtSettings
        {
            JwtKey = "secret_key_at_least_32_chars_long_!!",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            Lifetime = "60"
        });
        Service = new UserTokenService(settings, Mock.Of<IUnitOfWork>(), TimeProvider);
    }
}
