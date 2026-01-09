using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Interfaces.Repositories.Base;

namespace OrderPaymentSystem.Domain.Interfaces.Repositories;

public interface IUserRoleRepository : IBaseRepository<UserRole>
{
    Task<UserRole> GetByUserIdAndRoleIdAsync(Guid userId, int roleId, CancellationToken cancellationToken = default);
}
