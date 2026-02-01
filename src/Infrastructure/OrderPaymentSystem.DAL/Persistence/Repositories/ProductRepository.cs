using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.DAL.Persistence.Repositories.Base;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Interfaces.Repositories;

namespace OrderPaymentSystem.DAL.Persistence.Repositories;

internal class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<IReadOnlyDictionary<int, Product>> GetProductsAsDictionaryByIdAsync(IEnumerable<int> productIds, CancellationToken cancellationToken = default)
    {
        if (productIds == null || !productIds.Any())
        {
            return new Dictionary<int, Product>();
        }

        var distinctIds = productIds.Distinct().ToList();

        return await _table
            .AsNoTracking()
            .Where(x => distinctIds.Contains(x.Id))
            .ToDictionaryAsync(k => k.Id, v => v, cancellationToken);
    }
}
