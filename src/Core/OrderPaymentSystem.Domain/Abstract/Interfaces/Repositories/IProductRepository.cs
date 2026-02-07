using OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories.Base;
using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories;

public interface IProductRepository : IBaseRepository<Product>
{
    /// <summary>
    /// Получает словарь продуктов, где ключ - Id товара, значение - сам продукт.
    /// Запрос выполняется без отслеживания изменений.
    /// </summary>
    /// <param name="productIds">Коллекция ID продуктов для поиска.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Словарь продуктов (только для чтения).</returns>
    Task<IReadOnlyDictionary<int, Product>> GetProductsAsDictionaryByIdAsync(IEnumerable<int> productIds, CancellationToken cancellationToken = default);
}
