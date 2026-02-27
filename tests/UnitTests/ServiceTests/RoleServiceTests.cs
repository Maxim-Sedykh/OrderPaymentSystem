using FluentAssertions;
using Moq;
using OrderPaymentSystem.Application.Constants;
using OrderPaymentSystem.Application.DTOs.Role;
using OrderPaymentSystem.Application.Services.Roles;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Shared.Specifications;
using OrderPaymentSystem.UnitTests.Configurations.Fixtures;

namespace OrderPaymentSystem.UnitTests.ServiceTests;

/// <summary>
/// Тесты сервиса <see cref="RoleService"/>
/// </summary>
public class RoleServiceTests
{
    private readonly RoleFixture _fixture;

    /// <summary>
    /// Конструктор. Инициализация фикстуры
    /// </summary>
    public RoleServiceTests() => _fixture = new RoleFixture();

    /// <summary>
    /// Создание роли должно инвалидировать глобальный кэш по ролям
    /// </summary>
    [Fact]
    public async Task CreateAsync_WhenNewRole_ShouldSaveAndInvalidateCache()
    {
        // Arrange
        var dto = new CreateRoleDto("Manager");
        _fixture.SetupRoleExistence(false)
                .SetupMapping<Role, RoleDto>(new RoleDto(1, "Manager"));

        // Act
        var result = await _fixture.Service.CreateAsync(dto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _fixture.VerifySaved();
        _fixture.VerifyCacheRemoved(CacheKeys.Role.All);
    }

    /// <summary>
    /// Получение всех ролей если они закэшированы - должно быть из кэша
    /// </summary>
    [Fact]
    public async Task GetAllAsync_WhenCached_ShouldReturnFromCache()
    {
        // Arrange
        var cachedRoles = new List<RoleDto> { new(1, "Admin") };
        _fixture.SetupCacheGet(CacheKeys.Role.All, cachedRoles);

        // Act
        var result = await _fixture.Service.GetAllAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEquivalentTo(cachedRoles);
        _fixture.RoleRepo.Verify(r => r.GetListProjectedAsync<RoleDto>(It.IsAny<BaseSpecification<Role>>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
