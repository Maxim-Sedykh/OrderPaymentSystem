using FluentAssertions;
using Moq;
using OrderPaymentSystem.Application.Constants;
using OrderPaymentSystem.Application.DTOs.UserRole;
using OrderPaymentSystem.Application.Services.Roles;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.UnitTests.Configurations.Factories;
using OrderPaymentSystem.UnitTests.Configurations.Fixtures;

namespace OrderPaymentSystem.UnitTests.ServiceTests;

/// <summary>
/// Тесты сервиса <see cref="UserRoleService"/>
/// </summary>
public class UserRoleServiceTests
{
    private readonly UserRoleFixture _fixture;

    /// <summary>
    /// Конструктор. Инициализация фикстуры
    /// </summary>
    public UserRoleServiceTests() => _fixture = new UserRoleFixture();

    /// <summary>
    /// Добавление роли для пользователя, у которого уже есть эта роль - должна быть с ошибкой
    /// </summary>
    [Fact]
    public async Task CreateAsync_WhenUserAlreadyHasRole_ShouldReturnError()
    {
        // Arrange
        var user = TestDataFactory.User.Build();
        var role = TestDataFactory.Role.WithId(1).WithName("test").Build();

        _fixture.SetupUser(user)
                .SetupRole(role)
                .SetupUserExistingRoles([role.Name]);

        // Act
        var result = await _fixture.Service.CreateAsync(user.Id, role.Name);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(DomainErrors.Role.UserAlreadyHasRole(role.Name));
        _fixture.VerifyNotSaved();
    }

    /// <summary>
    /// Обновление роли у пользователя когда всё валидно - транзакция должна фиксироваться
    /// </summary>
    [Fact]
    public async Task UpdateAsync_WhenValid_ShouldCommitTransaction()
    {
        // Arrange
        var user = TestDataFactory.User.Build();
        var oldRole = TestDataFactory.Role.WithId(1).Build();
        var newRole = TestDataFactory.Role.WithId(2).Build();
        user.AddRoles(oldRole);

        _fixture.SetupUser(user)
                .SetupRole(newRole)
                .SetupUserRoleEntity(UserRole.Create(user.Id, oldRole.Id));

        // Act
        var result = await _fixture.Service.UpdateAsync(user.Id, new UpdateUserRoleDto(oldRole.Id, newRole.Id));

        // Assert
        result.IsSuccess.Should().BeTrue();
        _fixture.Transaction.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        _fixture.VerifyCacheRemoved(CacheKeys.User.Roles(user.Id));
    }
}
