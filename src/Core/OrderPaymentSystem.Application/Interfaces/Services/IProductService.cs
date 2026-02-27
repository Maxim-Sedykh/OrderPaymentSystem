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
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Все товары</returns>
    Task<CollectionResult<ProductDto>> GetAllAsync(CancellationToken ct = default);

    /// <summary>
    /// Получение товара по идентификатору
    /// </summary>
    /// <param name="id">Id товара</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Товар</returns>
    Task<DataResult<ProductDto>> GetByIdAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Добавление товара
    /// </summary>
    /// <param name="dto">Модель для создания товара</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Созданный товар</returns>
    Task<DataResult<ProductDto>> CreateAsync(CreateProductDto dto, CancellationToken ct = default);

    /// <summary>
    /// Удаление товара по идентификатору
    /// </summary>
    /// <param name="id">Id товара</param>
    /// <param name="ct">Токен отмены операции</param>
    Task<BaseResult> DeleteByIdAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Обновление товара
    /// </summary>
    /// <param name="id">Id товара</param>
    /// <param name="dto">Модель для обновления товара</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Обновлённый товар</returns>
    Task<DataResult<ProductDto>> UpdateAsync(int id, UpdateProductDto dto, CancellationToken ct = default);
}
