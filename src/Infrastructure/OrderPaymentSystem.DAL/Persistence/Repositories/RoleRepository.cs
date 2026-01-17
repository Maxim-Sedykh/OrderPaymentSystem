using OrderPaymentSystem.DAL.Persistence.Repositories.Base;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Interfaces.Repositories;

namespace OrderPaymentSystem.DAL.Persistence.Repositories;

internal class RoleRepository : BaseRepository<Role>, IRoleRepository
{
    public RoleRepository(ApplicationDbContext dbContext) : base(dbContext) { }
}
