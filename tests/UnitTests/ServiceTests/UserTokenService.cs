using FluentAssertions;
using OrderPaymentSystem.UnitTests.Configurations.Factories;
using OrderPaymentSystem.UnitTests.Configurations.Fixtures;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xunit;

namespace OrderPaymentSystem.UnitTests.ServiceTests;

public class UserTokenServiceTests
{
    private readonly TokenFixture _fixture;

    public UserTokenServiceTests() => _fixture = new TokenFixture();

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
