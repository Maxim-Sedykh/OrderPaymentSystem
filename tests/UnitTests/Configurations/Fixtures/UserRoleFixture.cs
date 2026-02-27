using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;
using OrderPaymentSystem.Application.Interfaces.Cache;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Application.Services.Roles;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Shared.Specifications;
using System.Linq.Expressions;

namespace OrderPaymentSystem.UnitTests.Configurations.Fixtures;

/// <summary>
/// Настройка зависимостей и создание <see cref="UserRoleService"/>
/// </summary>
internal class UserRoleFixture
{
    public Mock<IUnitOfWork> Uow { get; } = new();
    public Mock<IUserRepository> UserRepo { get; } = new();
    public Mock<IRoleRepository> RoleRepo { get; } = new();
    public Mock<IUserRoleRepository> UserRoleRepo { get; } = new();
    public Mock<ICacheService> Cache { get; } = new();
    public Mock<IDbContextTransaction> Transaction { get; } = new();
    public UserRoleService Service { get; }

    public UserRoleFixture()
    {
        Uow.Setup(u => u.Users).Returns(UserRepo.Object);
        Uow.Setup(u => u.Roles).Returns(RoleRepo.Object);
        Uow.Setup(u => u.UserRoles).Returns(UserRoleRepo.Object);
        Uow.Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Transaction.Object);
        Service = new UserRoleService(Uow.Object, Mock.Of<ILogger<UserRoleService>>(), Cache.Object);
    }

    public UserRoleFixture SetupUser(User user) 
    { 
        UserRepo.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<User>>(), It.IsAny<CancellationToken>())).ReturnsAsync(user); 

        return this; 
    }
    public UserRoleFixture SetupRole(Role role) 
    { 
        RoleRepo.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Role>>(), It.IsAny<CancellationToken>())).ReturnsAsync(role); 

        return this; 
    }
    public UserRoleFixture SetupUserExistingRoles(List<string> roleNames) 
    { 
        RoleRepo.Setup(r => r.GetListValuesAsync(It.IsAny<BaseSpecification<Role>>(),
            It.IsAny<Expression<Func<Role, string>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(roleNames); 

        return this; 
    }

    public UserRoleFixture SetupUserRoleEntity(UserRole ur) 
    { 
        UserRoleRepo.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<UserRole>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ur); 

        return this; 
    }

    public void VerifyNotSaved() => Uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    public void VerifyCacheRemoved(string key) => Cache.Verify(c => c.RemoveAsync(key, It.IsAny<CancellationToken>()), Times.Once);
}
