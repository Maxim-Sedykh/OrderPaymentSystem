using FluentAssertions;
using OrderPaymentSystem.Application.Services.Auth;
using OrderPaymentSystem.UnitTests.Configurations.Factories;
using OrderPaymentSystem.UnitTests.Configurations.Fixtures;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace OrderPaymentSystem.UnitTests.ServiceTests;

/// <summary>
/// Тесты сервиса <see cref="UserTokenService"/>
/// </summary>
public class UserTokenServiceTests
{
    private readonly TokenFixture _fixture;

    /// <summary>
    /// Конструктор. Инициализация фикстуры
    /// </summary>
    public UserTokenServiceTests() => _fixture = new TokenFixture();

    /// <summary>
    /// Генерация Access-токена должна иметь валидные данные
    /// Чтобы из него можно быть извлечь Claims и Issuer
    /// </summary>
    [Fact]
    public void GenerateAccessToken_ShouldHaveCorrectClaimsAndIssuer()
    {
        // Arrange
        var claims = new List<Claim> { new(ClaimTypes.Name, "test") };

        // Act
        var token = _fixture.Service.GenerateAccessToken(claims);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);
        jsonToken.Issuer.Should().Be("TestIssuer");
        jsonToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == "test");
    }

    /// <summary>
    /// Получение клеймов из пользователя.
    /// В клеймах пользователя должны быть все его роли
    /// </summary>
    [Fact]
    public void GetClaimsFromUser_WhenUserHasRoles_ShouldReturnAllClaims()
    {
        // Arrange
        var user = TestDataFactory.User.WithLogin("dev").Build();
        user.AddRoles(TestDataFactory.Role.WithName("Admin").Build());

        // Act
        var result = _fixture.Service.GetClaimsFromUser(user);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "Admin");
        result.Data.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == "dev");
    }
}
