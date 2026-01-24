using OrderPaymentSystem.DAL.Persistence.Repositories.Base;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Interfaces.Repositories;

namespace OrderPaymentSystem.DAL.Persistence.Repositories;

internal class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public Task<IReadOnlyDictionary<int, Product>> GetProductsAsDictionaryByIdAsync(IEnumerable<int> productIds, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
