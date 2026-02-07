using OrderPaymentSystem.DAL.Persistence.Repositories.Base;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.DAL.Persistence.Repositories;

internal class UserTokenRepository : BaseRepository<UserToken>, IUserTokenRepository
{
    public UserTokenRepository(ApplicationDbContext dbContext) : base(dbContext) { }
}
