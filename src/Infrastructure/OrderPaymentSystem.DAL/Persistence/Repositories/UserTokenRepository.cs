using OrderPaymentSystem.DAL.Persistence.Repositories.Base;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Interfaces.Repositories.Base;

namespace OrderPaymentSystem.DAL.Persistence.Repositories;

internal class UserTokenRepository : BaseRepository<UserToken>, IUserTokenRepository
{
    public UserTokenRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public Task<UserToken> CreateAsync(UserToken entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void Remove(UserToken entity)
    {
        throw new NotImplementedException();
    }

    public void RemoveRange(IEnumerable<UserToken> entities)
    {
        throw new NotImplementedException();
    }

    public UserToken Update(UserToken entity)
    {
        throw new NotImplementedException();
    }

    public void UpdateRange(IEnumerable<UserToken> entities)
    {
        throw new NotImplementedException();
    }

    IQueryable<UserToken> IBaseRepository<UserToken>.GetQueryable()
    {
        throw new NotImplementedException();
    }
}
