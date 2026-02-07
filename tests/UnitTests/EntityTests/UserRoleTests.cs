using FluentAssertions;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Shared.Exceptions;
using Xunit;

namespace OrderPaymentSystem.UnitTests.EntityTests;

public class UserRoleTests
{
    [Fact]
    public void Create_ValidData_ShouldSetProperties()
    {
        // Arrange
        var userId = Guid.NewGuid();
        const int roleId = 1;

        // Act
        var userRole = UserRole.Create(userId, roleId);

        // Assert
        userRole.UserId.Should().Be(userId);
        userRole.RoleId.Should().Be(roleId);
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000", 1)]
    [InlineData("b2d7b42a-a9a2-4a7b-a4b2-2b8b9b8b9b8b", 0)]
    public void Create_InvalidData_ShouldThrowBusinessException(string userIdString, int roleId)
    {
        // Arrange
        var userId = Guid.Parse(userIdString);

        // Act
        Action act = () => UserRole.Create(userId, roleId);

        // Assert
        act.Should().Throw<BusinessException>();
    }
}
