using OrderPaymentSystem.Domain.Dto.Product;
using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Domain.Interfaces.Services;

/// <summary>
/// Интерфейс сервиса отвечающий за работу с доменной части товаров (Product)
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Получение всех товаров
    /// </summary>
    /// <returns></returns>
    Task<CollectionResult<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение товара по идентификатору
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<DataResult<ProductDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавление товара
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<BaseResult> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаление товара по идентификатору
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<BaseResult> DeleteByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновление товара
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<DataResult<ProductDto>> UpdateAsync(int id, UpdateProductDto dto, CancellationToken cancellationToken = default);
}
