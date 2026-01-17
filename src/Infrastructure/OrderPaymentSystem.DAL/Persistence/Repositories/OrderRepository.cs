using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.DAL.Persistence.Repositories.Base;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Interfaces.Repositories;

namespace OrderPaymentSystem.DAL.Persistence.Repositories;

internal class OrderRepository : BaseRepository<Order>, IOrderRepository
{
    public OrderRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<bool> ExistsByIdAsync(long orderId, CancellationToken cancellationToken = default)
    {
        return await _table.AnyAsync(x => x.Id == orderId, cancellationToken);
    }

    public async Task<Order> GetByIdAsync(long orderId, CancellationToken cancellationToken = default)
    {
        return await _table.FirstOrDefaultAsync(x => x.Id == orderId, cancellationToken);
    }

    public IQueryable<Order> GetByIdQuery(long orderId)
    {
        return _table.Where(x => x.Id == orderId);
    }

    public Task<Order> GetByIdWithAllDetailsAsync(long orderId, CancellationToken cancellationToken = default)
    {
        return _table
            .Include(x => x.Items)
            .ThenInclude(x => x.Product)
            .Include(x => x.Payment)
            .FirstOrDefaultAsync();
    }

    public Task<Order> GetByIdWithItemsAndProductsAsync(long orderId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Order> GetByIdWithItemsAsync(long orderId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public IQueryable<Order> GetByUserIdQuery(Guid userId)
    {
        throw new NotImplementedException();
    }
}
