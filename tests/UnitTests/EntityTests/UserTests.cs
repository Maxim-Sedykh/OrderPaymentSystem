using FluentAssertions;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Shared.Exceptions;

namespace OrderPaymentSystem.UnitTests.EntityTests;

/// <summary>
/// Тесты сущности <see cref="User"/>
/// </summary>
public class UserTests
{
    /// <summary>
    /// Создание пользователя с валидными данными должно быть успешно
    /// </summary>
    [Fact]
    public void Create_ValidData_ShouldReturnUserWithGeneratedId()
    {
        // Act
        var user = User.Create("testlogin", "hashedpassword");

        // Assert
        user.Login.Should().Be("testlogin");
        user.PasswordHash.Should().Be("hashedpassword");
        user.Id.Should().NotBe(Guid.Empty);
    }

    /// <summary>
    /// Создание пользователя с невалидными данными должно вызывать BusinessException
    /// </summary>
    [Theory]
    [InlineData(null, "pass")]
    [InlineData("", "pass")]
    [InlineData("login", null)]
    [InlineData("login", "")]
    public void Create_InvalidData_ShouldThrowBusinessException(string? login, string? passwordHash)
    {
        // Act
        Action act = () => User.Create(login, passwordHash);

        // Assert
        act.Should().Throw<BusinessException>();
    }

    /// <summary>
    /// Смена пароля с новым валидным паролем должна обновить пароль
    /// </summary>
    [Fact]
    public void ChangePassword_ValidNewPassword_ShouldUpdatePasswordHash()
    {
        // Arrange
        var user = User.Create("test", "oldhash");

        // Act
        user.ChangePassword("newhash");

        // Assert
        user.PasswordHash.Should().Be("newhash");
    }
    
    /// <summary>
    /// Смена пароля на инвалидный пароль должна вызывать BusinessException
    /// </summary>
    /// <param name="newPasswordHash"></param>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ChangePassword_InvalidNewPassword_ShouldThrowBusinessException(string? newPasswordHash)
    {
        // Arrange
        var user = User.Create("test", "oldhash");

        // Act
        Action act = () => user.ChangePassword(newPasswordHash);

        // Assert
        act.Should().Throw<BusinessException>();
    }
}
