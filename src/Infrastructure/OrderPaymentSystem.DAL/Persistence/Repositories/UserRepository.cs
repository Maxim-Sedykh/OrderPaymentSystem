using OrderPaymentSystem.DAL.Persistence.Repositories.Base;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.DAL.Persistence.Repositories;

internal class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext dbContext) : base(dbContext) { }
}
