using MapsterMapper;
using Moq;
using OrderPaymentSystem.Application.Interfaces.Cache;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Application.Services.Roles;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Shared.Specifications;

namespace OrderPaymentSystem.UnitTests.Configurations.Fixtures;

internal class RoleFixture
{
    public Mock<IUnitOfWork> Uow { get; } = new();
    public Mock<IRoleRepository> RoleRepo { get; } = new();
    public Mock<ICacheService> Cache { get; } = new();
    public Mock<IMapper> Mapper { get; } = new();
    public RoleService Service { get; }

    public RoleFixture()
    {
        Uow.Setup(u => u.Roles).Returns(RoleRepo.Object);
        Service = new RoleService(Uow.Object, Mock.Of<Microsoft.Extensions.Logging.ILogger<RoleService>>(), Cache.Object, Mapper.Object);
    }

    public RoleFixture SetupRole(Role role)
    {
        RoleRepo.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Role>>(), It.IsAny<CancellationToken>())).ReturnsAsync(role);
        return this;
    }

    public RoleFixture SetupRoleExistence(bool exists)
    {
        RoleRepo.Setup(r => r.AnyAsync(It.IsAny<BaseSpecification<Role>>(), It.IsAny<CancellationToken>())).ReturnsAsync(exists);
        return this;
    }

    public RoleFixture SetupCacheGet<T>(string key, T value) where T : class
    {
        Cache.Setup(c => c.GetOrCreateAsync(key, It.IsAny<Func<CancellationToken, Task<T>>>())).ReturnsAsync(value);
        return this;
    }

    public RoleFixture SetupMapping<TS, TD>(TD dest)
    {
        Mapper.Setup(m => m.Map<TD>(It.IsAny<TS>())).Returns(dest);
        return this;
    }

    public void VerifySaved() => Uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    public void VerifyNotSaved() => Uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    public void VerifyCacheRemoved(string key) => Cache.Verify(c => c.RemoveAsync(key, It.IsAny<CancellationToken>()), Times.Once);
}
