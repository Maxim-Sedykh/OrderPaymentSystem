using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Interfaces.Repositories.Base;

namespace OrderPaymentSystem.Domain.Interfaces.Repositories;

public interface IProductRepository : IBaseRepository<Product>
{
    Task<Product> GetByIdAsync(int id, bool asNoTracking = false, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
    IQueryable<Product> GetByIdQuery(int id);
    IQueryable<Product> GetAllQuery();

    /// <summary>
    /// Получает словарь продуктов, где ключ - Id товара, значение - сам продукт.
    /// Запрос выполняется без отслеживания изменений.
    /// </summary>
    /// <param name="productIds">Коллекция ID продуктов для поиска.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Словарь продуктов (только для чтения).</returns>
    Task<IReadOnlyDictionary<int, Product>> GetProductsAsDictionaryByIdAsync(IEnumerable<int> productIds, CancellationToken cancellationToken = default);
}
