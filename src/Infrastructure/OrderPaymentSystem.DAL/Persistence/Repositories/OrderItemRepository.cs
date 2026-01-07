using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.DAL.Persistence.Repositories.Base;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Interfaces.Repositories;

namespace OrderPaymentSystem.DAL.Persistence.Repositories;

public class OrderItemRepository : BaseRepository<OrderItem>, IOrderItemRepository
{
    public OrderItemRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<OrderItem> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _table.FindAsync(id, cancellationToken);
    }

    public async Task<OrderItem> GetByIdWithProductAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _table.AsQueryable()
            .Include(x => x.Product)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public IQueryable<OrderItem> GetByOrderId(long orderId)
    {
        return _table.AsQueryable()
            .Where(x => x.OrderId == orderId);
    }
}
