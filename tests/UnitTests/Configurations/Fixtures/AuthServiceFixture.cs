using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using Moq;
using OrderPaymentSystem.Application.Interfaces.Auth;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Application.Interfaces.Services;
using OrderPaymentSystem.Application.Services.Auth;
using OrderPaymentSystem.Application.Settings;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Shared.Result;
using OrderPaymentSystem.Shared.Specifications;
using System.Linq.Expressions;
using System.Security.Claims;

namespace OrderPaymentSystem.UnitTests.Configurations.Fixtures;

/// <summary>
/// Настройка зависимостей и создание <see cref="AuthService"/>
/// </summary>
internal class AuthServiceFixture
{
    public Mock<IUnitOfWork> Uow { get; } = new();
    public Mock<IUserRepository> UserRepo { get; } = new();
    public Mock<IRoleRepository> RoleRepo { get; } = new();
    public Mock<IUserTokenRepository> TokenRepo { get; } = new();
    public Mock<IUserRoleRepository> UserRoleRepo { get; } = new();
    public Mock<IPasswordHasher> Hasher { get; } = new();
    public Mock<IUserTokenService> TokenService { get; } = new();
    public FakeTimeProvider TimeProvider { get; } = new();
    public Mock<IDbContextTransaction> Transaction { get; } = new();

    public AuthService Service { get; }

    public AuthServiceFixture()
    {
        Uow.Setup(u => u.Users).Returns(UserRepo.Object);
        Uow.Setup(u => u.Roles).Returns(RoleRepo.Object);
        Uow.Setup(u => u.UserTokens).Returns(TokenRepo.Object);
        Uow.Setup(u => u.UserRoles).Returns(UserRoleRepo.Object);
        Uow.Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Transaction.Object);

        var jwtOptions = Options.Create(new JwtSettings { RefreshTokenValidityInDays = 7 });

        Service = new AuthService(
            Mock.Of<ILogger<AuthService>>(),
            TokenService.Object,
            Uow.Object,
            Hasher.Object,
            TimeProvider,
            jwtOptions);
    }

    public AuthServiceFixture SetupUserByLogin(User? user)
    {
        UserRepo.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<User>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
        return this;
    }

    public AuthServiceFixture SetupPasswordVerification(bool isValid)
    {
        Hasher.Setup(h => h.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(isValid);
        return this;
    }

    public AuthServiceFixture SetupTokenGeneration(string access, string refresh)
    {
        TokenService.Setup(s => s.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>())).Returns(access);
        TokenService.Setup(s => s.GenerateRefreshToken()).Returns(refresh);
        return this;
    }

    public AuthServiceFixture SetupClaims(CollectionResult<Claim> result)
    {
        TokenService.Setup(s => s.GetClaimsFromUser(It.IsAny<User>())).Returns(result);
        return this;
    }

    public AuthServiceFixture SetupUserExistence(bool exists)
    {
        UserRepo.Setup(r => r.AnyAsync(It.IsAny<BaseSpecification<User>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(exists);
        return this;
    }

    public AuthServiceFixture SetupRoleSearch(int roleId)
    {
        RoleRepo.Setup(r => r.GetValueAsync(It.IsAny<BaseSpecification<Role>>(), It.IsAny<Expression<Func<Role, int>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(roleId);
        return this;
    }

    public void VerifySaved() => Uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    public void VerifyNotSaved() => Uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    public void VerifyTransactionRollback() => Transaction.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
}
