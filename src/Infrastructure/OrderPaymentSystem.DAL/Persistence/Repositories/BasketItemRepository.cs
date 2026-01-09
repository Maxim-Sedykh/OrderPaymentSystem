using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.DAL.Persistence.Repositories.Base;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Interfaces.Repositories;

namespace OrderPaymentSystem.DAL.Persistence.Repositories;

public class BasketItemRepository : BaseRepository<BasketItem>, IBasketItemRepository
{
    public BasketItemRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<BasketItem> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _table.FindAsync([ id ], cancellationToken: cancellationToken);
    }

    public async Task<BasketItem> GetByIdWithProductAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _table.AsQueryable()
            .Include(x => x.Product)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public IQueryable<BasketItem> GetByUserIdQuery(Guid userId)
    {
        return _table.Where(x => x.UserId == userId);
    }
}
