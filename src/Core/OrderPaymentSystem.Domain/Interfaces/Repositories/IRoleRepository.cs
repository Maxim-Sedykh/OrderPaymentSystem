using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Interfaces.Repositories.Base;

namespace OrderPaymentSystem.Domain.Interfaces.Repositories;

public interface IRoleRepository : IBaseRepository<Role>
{
    IQueryable<Role> GetByUserIdQuery(Guid userId);

    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);

    Task<Role> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    IQueryable<Role> GetAllQuery();

    Task<int> GetRoleIdByNameAsync(string name, CancellationToken cancellationToken = default);
}
