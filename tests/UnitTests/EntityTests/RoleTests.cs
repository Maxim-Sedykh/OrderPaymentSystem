using FluentAssertions;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Shared.Exceptions;

namespace OrderPaymentSystem.UnitTests.EntityTests;

/// <summary>
/// Тесты сущности <see cref="Role"/>
/// </summary>
public class RoleTests
{
    /// <summary>
    /// Обновление роли с валидным именем должно быть успешно
    /// </summary>
    [Fact]
    public void Create_ValidName_ShouldSetProperties()
    {
        // Act
        var role = Role.Create("Admin");

        // Assert
        role.Name.Should().Be("Admin");
        role.Id.Should().Be(default);
    }

    /// <summary>
    /// Создание роли с невалидным именем должно выбрасывать BusinessException
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_InvalidName_ShouldThrowBusinessException(string? name)
    {
        // Act
        Action act = () => Role.Create(name);

        // Assert
        act.Should().Throw<BusinessException>();
    }

    /// <summary>
    /// Обновление роли с валидным именем должно быть успешно
    /// </summary>
    [Fact]
    public void UpdateName_ValidName_ShouldUpdateProperty()
    {
        // Arrange
        var role = Role.Create("OldName");

        // Act
        role.UpdateName("NewName");

        // Assert
        role.Name.Should().Be("NewName");
    }

    /// <summary>
    /// Обновление роли с невалидным именем должно выбрасывать BusinessException
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateName_InvalidNewName_ShouldThrowBusinessException(string? newName)
    {
        // Arrange
        var role = Role.Create("ValidRole");

        // Act
        Action act = () => role.UpdateName(newName);

        // Assert
        act.Should().Throw<BusinessException>();
    }
}
