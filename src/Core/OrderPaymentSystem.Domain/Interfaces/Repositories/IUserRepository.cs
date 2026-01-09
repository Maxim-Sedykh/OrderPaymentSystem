using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Interfaces.Repositories.Base;

namespace OrderPaymentSystem.Domain.Interfaces.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User> GetByIdWithRolesAsync(Guid id, CancellationToken cancellationToken = default);

    Task<User> GetWithRolesAndTokenByLoginAsync(string login, CancellationToken cancellationToken = default);
    Task<bool> ExistsByLoginAsync(string login, CancellationToken cancellationToken);
}
