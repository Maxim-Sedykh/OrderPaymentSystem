using OrderPaymentSystem.DAL.Persistence.Repositories.Base;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Interfaces.Repositories.Base;

namespace OrderPaymentSystem.DAL.Persistence.Repositories;

internal class UserRoleRepository : BaseRepository<UserRole>, IUserRoleRepository
{
    public UserRoleRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public Task<UserRole> GetByUserIdAndRoleIdAsync(Guid userId, int roleId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
