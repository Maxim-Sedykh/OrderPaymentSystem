using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using OrderPaymentSystem.Application.DTOs.Token;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Application.Interfaces.Services;
using OrderPaymentSystem.Application.Services;
using OrderPaymentSystem.Application.Settings;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Domain.Resources;
using OrderPaymentSystem.Shared.Result;
using Microsoft.Extensions.Time.Testing;
using OrderPaymentSystem.Shared.Specifications;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xunit;

namespace OrderPaymentSystem.UnitTests.ServiceTests
{
    public class UserTokenServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IOptions<JwtSettings>> _jwtOptionsMock;
        private readonly Mock<ILogger<UserTokenService>> _loggerMock; // Добавлено
        private readonly FakeTimeProvider _timeProvider;
        private readonly UserTokenService _userTokenService;
        private readonly JwtSettings _jwtSettings;

        private const string TestJwtKey = "super_secret_key_for_testing_purpose_that_is_at_least_16_bytes_long"; // Должен быть >= 16 байт
        private const string TestIssuer = "TestIssuer";
        private const string TestAudience = "TestAudience";

        public UserTokenServiceTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<UserTokenService>>(); // Инициализация логгера

            _jwtSettings = new JwtSettings
            {
                JwtKey = TestJwtKey,
                Issuer = TestIssuer,
                Audience = TestAudience,
                Lifetime = "10" // Minutes for access token
            };
            _jwtOptionsMock = new Mock<IOptions<JwtSettings>>();
            _jwtOptionsMock.Setup(opt => opt.Value).Returns(_jwtSettings);

            _timeProvider = new FakeTimeProvider();
            var initialTime = new DateTimeOffset(2023, 1, 1, 12, 0, 0, TimeSpan.Zero);
            _timeProvider.SetUtcNow(initialTime);

            _userTokenService = new UserTokenService(_jwtOptionsMock.Object, _uowMock.Object, _timeProvider);
        }

        [Fact]
        public void GenerateAccessToken_WithClaims_ShouldReturnValidToken()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            };

            // Act
            var token = _userTokenService.GenerateAccessToken(claims);

            // Assert
            token.Should().NotBeNullOrEmpty();
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            jwtToken.Issuer.Should().Be(TestIssuer);
            jwtToken.Audiences.Should().Contain(TestAudience);
            jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == "testuser");
        }

        [Fact]
        public void GenerateRefreshToken_ShouldReturnValidToken()
        {
            // Act
            var token = _userTokenService.GenerateRefreshToken();

            // Assert
            token.Should().NotBeNullOrEmpty();
            token.Should().BeOfType<string>();
            token.Length.Should().BeGreaterThan(32); // Base64 encoded token from 32 bytes
        }

        [Fact]
        public void GetClaimsFromUser_WithValidUserAndRoles_ShouldReturnClaims()
        {
            // Arrange
            var user = User.CreateExisting(Guid.NewGuid(), "testuser", "hashed_password");
            // Добавляем роли, чтобы тест прошел
            var role1 = Role.CreateExisting(1, "Admin");
            var role2 = Role.CreateExisting(2, "User");
            user.AddRole(role1); user.AddRole(role2);

            // Act
            var claims = _userTokenService.GetClaimsFromUser(user);

            // Assert
            claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == user.Login);
            claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == user.Id.ToString());
            claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == role1.Name);
            claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == role2.Name);
        }

        [Fact]
        public void GetClaimsFromUser_UserWithoutRoles_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var user = User.CreateExisting(Guid.NewGuid(), "testuser", "hashed_password");
            // Пользователь без ролей

            // Act
            Action act = () => _userTokenService.GetClaimsFromUser(user);

            // Assert
            act.Should().Throw<InvalidOperationException>().WithMessage(ErrorMessage.UserRolesNotFound);
        }
    }
}
