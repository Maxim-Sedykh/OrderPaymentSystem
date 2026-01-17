using OrderPaymentSystem.DAL.Persistence.Repositories.Base;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Interfaces.Repositories;

namespace OrderPaymentSystem.DAL.Persistence.Repositories;

internal class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public Task<bool> ExistsByLoginAsync(string login, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<User> GetByIdWithRolesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<User> GetWithRolesAndTokenByLoginAsync(string login, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
