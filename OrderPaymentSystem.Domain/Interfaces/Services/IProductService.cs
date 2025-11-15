using OrderPaymentSystem.Domain.Dto.Product;
using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Domain.Interfaces.Services;

/// <summary>
/// Сервис отвечающий за работу с доменной части товаров (Product)
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Получение всех товаров
    /// </summary>
    /// <returns></returns>
    Task<CollectionResult<ProductDto>> GetProductsAsync();

    /// <summary>
    /// Получение товара по идентификатору
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<BaseResult<ProductDto>> GetProductByIdAsync(int id);

    /// <summary>
    /// Добавление товара
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<BaseResult<ProductDto>> CreateProductAsync(CreateProductDto dto);

    /// <summary>
    /// Удаление товара по идентификатору
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<BaseResult<ProductDto>> DeleteProductAsync(int id);

    /// <summary>
    /// Обновление товара
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<BaseResult<ProductDto>> UpdateProductAsync(UpdateProductDto dto);
}
