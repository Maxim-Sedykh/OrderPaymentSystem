using OrderPaymentSystem.DAL.Persistence.Repositories.Base;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.DAL.Persistence.Repositories;

internal class UserRoleRepository : BaseRepository<UserRole>, IUserRoleRepository
{
    public UserRoleRepository(ApplicationDbContext dbContext) : base(dbContext) { }
}
