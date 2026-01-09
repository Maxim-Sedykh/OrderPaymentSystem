using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Interfaces.Repositories.Base;

namespace OrderPaymentSystem.Domain.Interfaces.Repositories;

public interface IOrderItemRepository : IBaseRepository<OrderItem>
{
    Task<OrderItem> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<OrderItem> GetByIdWithProductAsync(long id, CancellationToken cancellationToken = default);

    IQueryable<OrderItem> GetByOrderId(long orderId);
}
