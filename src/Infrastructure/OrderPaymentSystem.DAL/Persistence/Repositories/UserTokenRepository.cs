using OrderPaymentSystem.DAL.Persistence.Repositories.Base;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Interfaces.Repositories;

namespace OrderPaymentSystem.DAL.Persistence.Repositories;

internal class UserTokenRepository : BaseRepository<UserToken>, IUserTokenRepository
{
    public UserTokenRepository(ApplicationDbContext dbContext) : base(dbContext) { }
}
