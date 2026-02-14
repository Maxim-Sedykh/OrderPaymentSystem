using FluentAssertions;
using Moq;
using OrderPaymentSystem.Application.Constants;
using OrderPaymentSystem.Application.DTOs.Role;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Shared.Specifications;
using OrderPaymentSystem.UnitTests.Configurations.Factories;
using OrderPaymentSystem.UnitTests.Configurations.Fixtures;
using Xunit;

namespace OrderPaymentSystem.UnitTests.ServiceTests;

public class RoleServiceTests
{
    private readonly RoleFixture _fixture;

    public RoleServiceTests() => _fixture = new RoleFixture();

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

    [Fact]
    public async Task UpdateAsync_WhenNameIsSame_ShouldReturnNoChangesError()
    {
        // Arrange
        var role = TestDataFactory.Role.WithName("Admin").Build();
        _fixture.SetupRole(role);

        // Act
        var result = await _fixture.Service.UpdateAsync(role.Id, new UpdateRoleDto("Admin"));

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(DomainErrors.General.NoChanges());
        _fixture.VerifyNotSaved();
    }

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
