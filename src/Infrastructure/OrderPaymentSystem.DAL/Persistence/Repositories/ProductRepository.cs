using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.DAL.Persistence.Repositories.Base;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.DAL.Persistence.Repositories;

/// <summary>
/// Репозиторий для работы с сущностью <see cref="Product"/>
/// </summary>
internal class ProductRepository : BaseRepository<Product>, IProductRepository
{
    /// <summary>
    /// Конструктор репозитория
    /// </summary>
    /// <param name="dbContext">Контекст для работы с БД</param>
    public ProductRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    /// <inheritdoc/>
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
