using FluentAssertions;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Shared.Exceptions;

namespace OrderPaymentSystem.UnitTests.EntityTests;

/// <summary>
/// Тесты сущности <see cref="UserToken"/>
/// </summary>
public class UserTokenTests
{
    /// <summary>
    /// Создание токена с временем истечения срока в прошлом должно вызывать BusinessException
    /// </summary>
    [Fact]
    public void Create_WithPastExpiry_ShouldThrowBusinessException()
    {
        // Act
        Action act = () => UserToken.Create(Guid.NewGuid(), "token", DateTime.UtcNow.AddMinutes(-1), DateTime.UtcNow);

        // Assert
        act.Should().Throw<BusinessException>();
    }
}
