using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Interfaces.Repositories.Base;

namespace OrderPaymentSystem.Domain.Interfaces.Repositories;

public interface IPaymentRepository : IBaseRepository<Payment>
{
    Task<Payment> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByOrderIdAsync(long orderId, CancellationToken cancellationToken = default);
    IQueryable<Payment> GetByIdQuery(long id);
    IQueryable<Payment> GetByOrderIdQuery(long orderId);
}
