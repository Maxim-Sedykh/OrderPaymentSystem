using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Interfaces.Repositories.Base;

namespace OrderPaymentSystem.Domain.Interfaces.Repositories;

public interface IOrderRepository : IBaseRepository<Order>
{
    Task<Order> GetByIdWithItemsAndProductsAsync(long orderId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByIdAsync(long orderId, CancellationToken cancellationToken = default);
    Task<Order> GetByIdWithItemsAsync(long orderId, CancellationToken cancellationToken = default);
    Task<Order> GetByIdAsync(long orderId, CancellationToken cancellationToken = default);
    Task<Order> GetByIdWithItemsAndProductsAndPaymentsAsync(long orderId, CancellationToken cancellationToken = default);

    IQueryable<Order> GetByIdQuery(long orderId);

    IQueryable<Order> GetByUserIdQuery(Guid userId);
}
