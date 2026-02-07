using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace OrderPaymentSystem.UnitTests.ServiceTests
{
    public class AuthServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<IUserTokenService> _userTokenServiceMock;
        private readonly Mock<ILogger<AuthService>> _loggerMock;
        private readonly AuthService _authService;
        private readonly JwtSettings _jwtSettings;
        private readonly Mock<IOptions<JwtSettings>> _jwtOptionsMock;
        private readonly TimeProvider _timeProvider = TimeProvider.Use().Build(); // Для контроля времени

        public AuthServiceTests()
        {
            _uowMock = AuthServiceMockConfigurations.SetupBasicUnitOfWork();
            _passwordHasherMock = AuthServiceMockConfigurations.SetupPasswordHasher();
            _userTokenServiceMock = AuthServiceMockConfigurations.SetupUserTokenService("valid_access_token", "valid_refresh_token");
            _loggerMock = new Mock<ILogger<AuthService>>();

            _jwtSettings = new JwtSettings { JwtKey = "super_secret_key_for_testing_purpose", Issuer = "TestIssuer", Audience = "TestAudience", JwtLifeTime = 10 }; // JwtLifeTime здесь не используется напрямую AuthService, но нужно для UserTokenService
            _jwtOptionsMock = new Mock<IOptions<JwtSettings>>();
            _jwtOptionsMock.Setup(opt => opt.Value).Returns(_jwtSettings);

            // Фиксируем время для тестов, связанных со временем
            _timeProvider.SetLocalNow(new DateTimeOffset(2023, 1, 1, 12, 0, 0, TimeSpan.Zero));

            _authService = new AuthService(
                _loggerMock.Object,
                _userTokenServiceMock.Object,
                _uowMock.Object,
                _passwordHasherMock.Object,
                _jwtOptionsMock.Object, // Добавлено
                _timeProvider); // Добавлено
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ShouldReturnTokenDto()
        {
            // Arrange
            var loginDto = new LoginUserDto { Login = "testuser", Password = "password123" };
            var user = User.Create("testuser", "hashed_password");
            user.Id = Guid.NewGuid(); // Устанавливаем ID
            user.UserToken = UserToken.Create(user.Id, "existing_refresh", _timeProvider.GetUtcNow().UtcDateTime.AddDays(7)); // Существующий токен

            // Мокируем репозитории
            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Specification<User>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _uowMock.Setup(uow => uow.Users).Returns(userRepositoryMock.Object);

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.AccessToken.Should().Be("valid_access_token");
            result.Data.RefreshToken.Should().Be("valid_refresh_token");

            // Проверяем, что SaveChangesAsync был вызван (для обновления токена)
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            // Проверяем, что UserToken был обновлен
            user.UserToken.RefreshToken.Should().Be("valid_refresh_token");
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
            var loginDto = new LoginUserDto { Login = "testuser", Password = "password123" };
            var user = User.Create("testuser", "hashed_password");
            user.Id = Guid.NewGuid();
            user.UserToken = null; // У пользователя нет токена

            var userTokenRepositoryMock = new Mock<IUserTokenRepository>();
            userTokenRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<UserToken>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _uowMock.Setup(uow => uow.UserToken).Returns(userTokenRepositoryMock.Object);

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Specification<User>>(), It.IsAny<CancellationToken>())).ReturnsAsync(user);
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
            var registerDto = new RegisterUserDto { Login = "newuser", Password = "password123" };
            var newUser = User.Create(registerDto.Login, "hashed_password");
            var defaultRoleId = 5;

            // Мокируем репозитории
            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(r => r.AnyAsync(It.IsAny<Specification<User>>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
            userRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).Callback<User, CancellationToken>((u, ct) => { /* Просто вызываем */ });
            _uowMock.Setup(uow => uow.Users).Returns(userRepositoryMock.Object);

            var roleRepositoryMock = new Mock<IRoleRepository>();
            roleRepositoryMock.Setup(r => r.GetValueAsync(It.IsAny<Specification<Role>>(), It.IsAny<Func<Role, int>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(defaultRoleId);
            _uowMock.Setup(uow => uow.Roles).Returns(roleRepositoryMock.Object);

            var userRoleRepositoryMock = new Mock<IUserRoleRepository>();
            userRoleRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<UserRole>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _uowMock.Setup(uow => uow.UserRoles).Returns(userRoleRepositoryMock.Object);

            // Act
            var result = await _authService.RegisterAsync(registerDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _uowMock.Verify(uow => uow.Users.CreateAsync(It.Is<User>(u => u.Login == registerDto.Login), It.IsAny<CancellationToken>()), Times.Once);
            _uowMock.Verify(uow => uow.UserRoles.CreateAsync(It.Is<UserRole>(ur => ur.RoleId == defaultRoleId), It.IsAny<CancellationToken>()), Times.Once);
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2)); // SaveChangesAsync вызывается дважды
            _uowMock.Verify(uow => uow.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            _uowMock.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_UserAlreadyExists_ShouldReturnFailure()
        {
            // Arrange
            var registerDto = new RegisterUserDto { Login = "existinguser", Password = "password123" };

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(r => r.AnyAsync(It.IsAny<Specification<User>>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
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
            var registerDto = new RegisterUserDto { Login = "newuser", Password = "password123" };
            var user = User.Create(registerDto.Login, "hashed_password");

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(r => r.AnyAsync(It.IsAny<Specification<User>>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
            userRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).Callback<User, CancellationToken>((u, ct) => { /* Просто вызываем */ });
            _uowMock.Setup(uow => uow.Users).Returns(userRepositoryMock.Object);

            var roleRepositoryMock = new Mock<IRoleRepository>();
            roleRepositoryMock.Setup(r => r.GetValueAsync(It.IsAny<Specification<Role>>(), It.IsAny<Func<Role, int>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(0); // Роль не найдена
            _uowMock.Setup(uow => uow.Roles).Returns(roleRepositoryMock.Object);

            // Act
            var result = await _authService.RegisterAsync(registerDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.Role.NotFoundByName(Role.DefaultUserRoleName));
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once); // SaveChangesAsync после создания пользователя
            _uowMock.Verify(uow => uow.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            _uowMock.Verify(uow => uow.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
            _uowMock.Verify(uow => uow.UserRoles.CreateAsync(It.IsAny<UserRole>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
