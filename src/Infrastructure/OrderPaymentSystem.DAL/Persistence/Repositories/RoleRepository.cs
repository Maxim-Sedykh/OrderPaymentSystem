using OrderPaymentSystem.DAL.Persistence.Repositories.Base;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.DAL.Persistence.Repositories;

internal class RoleRepository : BaseRepository<Role>, IRoleRepository
{
    public RoleRepository(ApplicationDbContext dbContext) : base(dbContext) { }
}
