using FluentAssertions;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Shared.Exceptions;
using Xunit;

namespace OrderPaymentSystem.UnitTests.EntityTests;

public class UserTokenTests
{
    [Fact]
    public void Create_WithPastExpiry_ShouldThrowBusinessException()
    {
        // Act
        Action act = () => UserToken.Create(Guid.NewGuid(), "token", DateTime.UtcNow.AddMinutes(-1));

        // Assert
        act.Should().Throw<BusinessException>();
    }
}
