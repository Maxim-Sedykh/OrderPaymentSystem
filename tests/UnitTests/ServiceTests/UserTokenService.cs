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
using OrderPaymentSystem.Shared.Result;
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
        private readonly TimeProvider _timeProvider;
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
                JwtLifeTime = 10 // Minutes for access token
            };
            _jwtOptionsMock = new Mock<IOptions<JwtSettings>>();
            _jwtOptionsMock.Setup(opt => opt.Value).Returns(_jwtSettings);

            _timeProvider = TimeProvider.Use().Build();
            _timeProvider.SetLocalNow(new DateTimeOffset(2023, 1, 1, 12, 0, 0, TimeSpan.Zero)); // Фиксируем время

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
            var user = User.Create("testuser", "hashed_password");
            user.Id = Guid.NewGuid();
            // Добавляем роли, чтобы тест прошел
            var role1 = Role.Create("Admin"); role1.Id = 1;
            var role2 = Role.Create("User"); role2.Id = 2;
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
            var user = User.Create("testuser", "hashed_password");
            user.Id = Guid.NewGuid();
            // Пользователь без ролей

            // Act
            Action act = () => _userTokenService.GetClaimsFromUser(user);

            // Assert
            act.Should().Throw<InvalidOperationException>().WithMessage(ErrorMessage.UserRolesNotFound);
        }

        [Fact]
        public async Task RefreshAsync_ValidTokens_ShouldReturnNewTokens()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = User.Create("testuser", "hashed_password");
            user.Id = userId;
            var currentTime = _timeProvider.GetUtcNow().UtcDateTime;
            var refreshToken = "valid_refresh_token_from_db";
            var expiryTime = currentTime.AddDays(7);
            user.UserToken = UserToken.Create(userId, refreshToken, expiryTime);

            // Мокируем репозитории
            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.Is<Specification<User>>(spec => spec.Predicate(user)), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _uowMock.Setup(uow => uow.Users).Returns(userRepositoryMock.Object);

            // Мокируем UserTokenService для генерации токенов
            var accessToken = "new_valid_access_token";
            var newRefreshToken = "new_valid_refresh_token";
            var userTokenServiceMock = new Mock<IUserTokenService>();
            userTokenServiceMock.Setup(uts => uts.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>())).Returns(accessToken);
            userTokenServiceMock.Setup(uts => uts.GenerateRefreshToken()).Returns(newRefreshToken);
            userTokenServiceMock.Setup(uts => uts.GetClaimsFromUser(user)).Returns(new List<Claim> { new Claim(ClaimTypes.Name, user.Login) });
            // Заменяем основной сервис на мок для проверки RefreshAsync
            // NOTE: Более чистый подход - вынести GetClaimsFromUser и Generate... в отдельные приватные методы,
            //       а UserTokenService.RefreshAsync должен был быть настроен для использования этих моков.
            //       Сейчас это немного громоздко. Для демонстрации просто заменим сам сервис.

            // Переопределяем UserTokenService для этой проверки
            var authServiceInstanceForRefresh = new AuthService(_loggerMock.Object, userTokenServiceMock.Object, _uowMock.Object, _passwordHasherMock.Object, _jwtOptionsMock.Object, _timeProvider);

            // Mock RefreshAsync чтобы не вызывать GetValidUserForRefreshAsync
            // Так как мы уже мокнули GetFirstOrDefaultAsync для User, можно было бы вызвать RefreshAsync напрямую.
            // Но для полноты, давайте мокнем GetValidUserForRefreshAsync
            var mockUserResult = DataResult<User>.Success(user);
            // Здесь нужен рефлекшн или вспомогательный метод, чтобы замокать приватный метод.
            // Проще будет сделать UserTokenService.RefreshAsync более тестируемым, вынеся GetValidUserForRefreshAsync в публичный или protected.
            // В данном случае, давайте предположим, что GetValidUserForRefreshAsync работает правильно и вернет user.

            // Act
            var result = await authServiceInstanceForRefresh.RefreshAsync(new TokenDto { AccessToken = "expired_access_token", RefreshToken = refreshToken });

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.AccessToken.Should().Be(accessToken);
            result.Data.RefreshToken.Should().Be(newRefreshToken);
            user.UserToken.RefreshToken.Should().Be(newRefreshToken); // Проверяем, что токен в БД обновился
            user.UserToken.RefreshTokenExpireTime.Should().BeCloseTo(currentTime.AddDays(7), TimeSpan.FromSeconds(1)); // Проверяем время экспирации
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RefreshAsync_InvalidRefreshToken_ShouldReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = User.Create("testuser", "hashed_password");
            user.Id = userId;
            var currentTime = _timeProvider.GetUtcNow().UtcDateTime;
            var refreshToken = "wrong_refresh_token_from_db"; // Не совпадает с DTO
            var expiryTime = currentTime.AddDays(7);
            user.UserToken = UserToken.Create(userId, refreshToken, expiryTime);

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Specification<User>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _uowMock.Setup(uow => uow.Users).Returns(userRepositoryMock.Object);

            // Act
            var result = await _userTokenService.RefreshAsync(new TokenDto { AccessToken = "any_access_token", RefreshToken = "dto_refresh_token" }, default);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.General.InvalidClientRequest());
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task RefreshAsync_ExpiredRefreshToken_ShouldReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = User.Create("testuser", "hashed_password");
            user.Id = userId;
            var currentTime = _timeProvider.GetUtcNow().UtcDateTime;
            var refreshToken = "valid_refresh_token";
            var expiryTime = currentTime.AddDays(-1); // Токен истек
            user.UserToken = UserToken.Create(userId, refreshToken, expiryTime);

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Specification<User>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _uowMock.Setup(uow => uow.Users).Returns(userRepositoryMock.Object);

            // Act
            var result = await _userTokenService.RefreshAsync(new TokenDto { AccessToken = "any_access_token", RefreshToken = refreshToken }, default);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.Token.RefreshExpired());
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public void GetPrincipalFromExpiredToken_InvalidToken_ShouldThrowSecurityTokenException()
        {
            // Arrange
            var invalidToken = "this.is.not.a.valid.jwt"; // Невалидный токен

            // Act
            Action act = () => _userTokenService.GetPrincipalFromExpiredToken(invalidToken); // Вызов приватного метода для теста

            // Assert
            act.Should().Throw<SecurityTokenException>();
        }
    }

}
