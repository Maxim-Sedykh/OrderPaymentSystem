using FluentAssertions;
using OrderPaymentSystem.Application.Services.Auth;
using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Shared.Result;
using OrderPaymentSystem.UnitTests.Configurations.Factories;
using OrderPaymentSystem.UnitTests.Configurations.Fixtures;
using System.Security.Claims;

namespace OrderPaymentSystem.UnitTests.ServiceTests;

/// <summary>
/// Тесты сервиса <see cref="AuthService"/>
/// </summary>
public class AuthServiceTests
{
    private readonly AuthServiceFixture _fixture;

    /// <summary>
    /// Конструктор. Инициализация фикстуры
    /// </summary>
    public AuthServiceTests()
    {
        _fixture = new AuthServiceFixture();
    }

    /// <summary>
    /// Если авторизация прошла успешно - должен вернуться Refresh и Access токен
    /// </summary>
    [Fact]
    public async Task LoginAsync_WhenCredentialsAreValid_ShouldReturnTokens()
    {
        // Arrange
        var user = TestDataFactory.User.Build();
        var loginDto = TestDataFactory.CreateLoginDto(user.Login);
        var expectedTokens = new { Access = "access_123", Refresh = "refresh_123" };

        _fixture.SetupUserByLogin(user)
                .SetupPasswordVerification(true)
                .SetupTokenGeneration(expectedTokens.Access, expectedTokens.Refresh)
                .SetupClaims(CollectionResult<Claim>.Success(new List<Claim>()));

        // Act
        var result = await _fixture.Service.LoginAsync(loginDto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.AccessToken.Should().Be(expectedTokens.Access);
        result.Data.RefreshToken.Should().Be(expectedTokens.Refresh);
        _fixture.VerifySaved();
    }

    /// <summary>
    /// Если данные пользователя не верны при авторизации - должна быть ошибка
    /// </summary>
    [Fact]
    public async Task LoginAsync_WhenUserNotFound_ShouldReturnInvalidCredentialsError()
    {
        // Arrange
        var loginDto = TestDataFactory.CreateLoginDto();
        _fixture.SetupUserByLogin(null);

        // Act
        var result = await _fixture.Service.LoginAsync(loginDto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(DomainErrors.User.InvalidCredentials());
        _fixture.VerifyNotSaved();
    }

    /// <summary>
    /// При регистрации - если пользователь с запрошенным логином уже существует - должна быть ошибка
    /// </summary>
    [Fact]
    public async Task RegisterAsync_WhenUserAlreadyExists_ShouldReturnFailure()
    {
        // Arrange
        var dto = TestDataFactory.CreateRegisterDto();
        _fixture.SetupUserExistence(true);

        // Act
        var result = await _fixture.Service.RegisterAsync(dto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(DomainErrors.User.AlreadyExist(dto.Login).Code);
        _fixture.VerifyNotSaved();
    }

    /// <summary>
    /// При регистрации - если роль "User" отсуствует в базе, транзакия должна откатиться
    /// </summary>
    [Fact]
    public async Task RegisterAsync_WhenDefaultRoleMissing_ShouldRollbackTransaction()
    {
        // Arrange
        var dto = TestDataFactory.CreateRegisterDto();
        _fixture.SetupUserExistence(false)
                .SetupRoleSearch(0);

        // Act
        var result = await _fixture.Service.RegisterAsync(dto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        _fixture.VerifyTransactionRollback();
    }
}
