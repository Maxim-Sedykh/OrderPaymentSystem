using FluentAssertions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using Moq;
using OrderPaymentSystem.Application.DTOs.Auth;
using OrderPaymentSystem.Application.Interfaces.Auth;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Application.Interfaces.Services;
using OrderPaymentSystem.Application.Services;
using OrderPaymentSystem.Application.Settings;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Shared.Specifications;
using OrderPaymentSystem.UnitTests.Configurations;
using System.Linq.Expressions;
using System.Security.Claims;
using Xunit;

namespace OrderPaymentSystem.UnitTests.ServiceTests
{
    public class AuthServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<IUserTokenService> _userTokenServiceMock;
        private readonly Mock<ILogger<AuthService>> _loggerMock;
        private readonly Mock<IDbContextTransaction> _transactionMock = new();
        private readonly AuthService _authService; // Для контроля времени

        public AuthServiceTests()
        {
            _uowMock = AuthServiceMockConfigurations.SetupBasicUnitOfWork();
            _passwordHasherMock = AuthServiceMockConfigurations.SetupPasswordHasher();
            _userTokenServiceMock = AuthServiceMockConfigurations.SetupUserTokenService("valid_access_token", "valid_refresh_token");
            _loggerMock = new Mock<ILogger<AuthService>>();

            _authService = new AuthService(
                _loggerMock.Object,
                _userTokenServiceMock.Object,
                _uowMock.Object,
                _passwordHasherMock.Object); // Добавлено
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ShouldReturnTokenDto()
        {
            var accessToken = "valid_access_token";
            var resreshToken = "valid_refresh_token";

            // Arrange
            var loginDto = new LoginUserDto("testuser", "password123");
            var user = User.CreateExisting(Guid.NewGuid(), "testuser", "hashed_password");
            user.SetToken(UserToken.Create(user.Id, "existing_refresh", DateTime.Now.AddDays(7), DateTime.UtcNow)); // Существующий токен
            _userTokenServiceMock.Setup(uts => uts.GenerateRefreshToken()).Returns(resreshToken);
            _userTokenServiceMock.Setup(uts => uts.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>())).Returns(accessToken);

			// Мокируем репозитории
			var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<User>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _uowMock.Setup(uow => uow.Users).Returns(userRepositoryMock.Object);

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.AccessToken.Should().Be(accessToken);
            result.Data.RefreshToken.Should().Be(resreshToken);

            // Проверяем, что SaveChangesAsync был вызван (для обновления токена)
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            // Проверяем, что UserToken был обновлен
            user.UserToken.RefreshToken.Should().Be(resreshToken);
        }

        [Fact]
        public async Task LoginAsync_InvalidCredentials_ShouldReturnFailure()
        {
            // Arrange
            var loginDto = new LoginUserDto("wronguser", "wrongpassword");
            // Мокируем репозиторий, чтобы он вернул null или неправильный пароль
            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<User>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null); // Пользователь не найден
            _uowMock.Setup(uow => uow.Users).Returns(userRepositoryMock.Object);

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.User.InvalidCredentials());
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_UserWithoutToken_ShouldCreateNewToken()
        {
            // Arrange
            var loginDto = new LoginUserDto("testuser", "password123");
            var user = User.CreateExisting(Guid.NewGuid(), "testuser", "hashed_password"); // У пользователя нет токена

            var userTokenRepositoryMock = new Mock<IUserTokenRepository>();
            _uowMock.Setup(uow => uow.UserToken).Returns(userTokenRepositoryMock.Object);

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<User>>(), It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _uowMock.Setup(uow => uow.Users).Returns(userRepositoryMock.Object);

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _uowMock.Verify(uow => uow.UserToken.CreateAsync(It.Is<UserToken>(ut => ut.UserId == user.Id), It.IsAny<CancellationToken>()), Times.Once);
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_NewUser_ShouldCreateUserAndRole()
        {
            // Arrange
            var registerDto = new RegisterUserDto("newuser", "testpass", "testpass");
            var newUser = User.Create(registerDto.Login, "hashed_password");
            var defaultRoleId = 5;

            // Мокируем репозитории
            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(r => r.AnyAsync(It.IsAny<BaseSpecification<User>>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
            userRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).Callback<User, CancellationToken>((u, ct) => { /* Просто вызываем */ });
            _uowMock.Setup(uow => uow.Users).Returns(userRepositoryMock.Object);
            _uowMock.Setup(uow => uow.BeginTransactionAsync(It.IsAny<CancellationToken>())).ReturnsAsync(_transactionMock.Object);

            var roleRepositoryMock = new Mock<IRoleRepository>();
            roleRepositoryMock.Setup(r => r.GetValueAsync(It.IsAny<BaseSpecification<Role>>(), It.IsAny<Expression<Func<Role, int>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(defaultRoleId);
            _uowMock.Setup(uow => uow.Roles).Returns(roleRepositoryMock.Object);

            var userRoleRepositoryMock = new Mock<IUserRoleRepository>();
            _uowMock.Setup(uow => uow.UserRoles).Returns(userRoleRepositoryMock.Object);

            // Act
            var result = await _authService.RegisterAsync(registerDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _uowMock.Verify(uow => uow.Users.CreateAsync(It.Is<User>(u => u.Login == registerDto.Login), It.IsAny<CancellationToken>()), Times.Once);
            _uowMock.Verify(uow => uow.UserRoles.CreateAsync(It.Is<UserRole>(ur => ur.RoleId == defaultRoleId), It.IsAny<CancellationToken>()), Times.Once);
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2)); // SaveChangesAsync вызывается дважды
            _uowMock.Verify(uow => uow.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
			_transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_UserAlreadyExists_ShouldReturnFailure()
        {
            // Arrange
            var registerDto = new RegisterUserDto("existinguser", "password123", "password123");


			var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(r => r.AnyAsync(It.IsAny<BaseSpecification<User>>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _uowMock.Setup(uow => uow.Users).Returns(userRepositoryMock.Object);

            // Act
            var result = await _authService.RegisterAsync(registerDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.User.AlreadyExist(registerDto.Login));
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            _uowMock.Verify(uow => uow.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task RegisterAsync_DefaultRoleNotFound_ShouldRollbackAndReturnFailure()
        {
			// Arrange
			var registerDto = new RegisterUserDto("existinguser", "password123", "password123");
			var user = User.Create(registerDto.Login, "hashed_password");

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(r => r.AnyAsync(It.IsAny<BaseSpecification<User>>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
            userRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).Callback<User, CancellationToken>((u, ct) => { /* Просто вызываем */ });
            _uowMock.Setup(uow => uow.Users).Returns(userRepositoryMock.Object);
			_uowMock.Setup(uow => uow.BeginTransactionAsync(It.IsAny<CancellationToken>())).ReturnsAsync(_transactionMock.Object);

			var roleRepositoryMock = new Mock<IRoleRepository>();
            roleRepositoryMock.Setup(r => r.GetValueAsync(It.IsAny<BaseSpecification<Role>>(), It.IsAny<Expression<Func<Role, int>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(0); // Роль не найдена
            _uowMock.Setup(uow => uow.Roles).Returns(roleRepositoryMock.Object);

            // Act
            var result = await _authService.RegisterAsync(registerDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.Role.NotFoundByName(Role.DefaultUserRoleName));
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once); // SaveChangesAsync после создания пользователя
            _uowMock.Verify(uow => uow.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
			_transactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
            _uowMock.Verify(uow => uow.UserRoles.CreateAsync(It.IsAny<UserRole>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
