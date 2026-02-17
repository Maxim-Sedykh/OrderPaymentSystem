using OrderPaymentSystem.Application.DTOs.Product;
using OrderPaymentSystem.Shared.Result;

namespace OrderPaymentSystem.Application.Interfaces.Services;

/// <summary>
/// Интерфейс сервиса отвечающий за работу с доменной части товаров (Product)
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Получение всех товаров
    /// </summary>
    /// <returns></returns>
    Task<CollectionResult<ProductDto>> GetAllAsync(CancellationToken ct = default);

    /// <summary>
    /// Получение товара по идентификатору
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<DataResult<ProductDto>> GetByIdAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Добавление товара
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<DataResult<ProductDto>> CreateAsync(CreateProductDto dto, CancellationToken ct = default);

    /// <summary>
    /// Удаление товара по идентификатору
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<BaseResult> DeleteByIdAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Обновление товара
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<DataResult<ProductDto>> UpdateAsync(int id, UpdateProductDto dto, CancellationToken ct = default);
}
