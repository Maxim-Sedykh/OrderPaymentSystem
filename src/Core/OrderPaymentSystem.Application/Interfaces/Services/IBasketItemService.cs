using OrderPaymentSystem.Application.DTOs;
using OrderPaymentSystem.Application.DTOs.Basket;
using OrderPaymentSystem.Shared.Result;

namespace OrderPaymentSystem.Application.Interfaces.Services;

/// <summary>
/// Интерфейс сервиса для работы с элементами корзины пользователя
/// </summary>
public interface IBasketItemService
{
    /// <summary>
    /// Создать элемент корзины пользователя
    /// </summary>
    /// <param name="userId">Id пользователя</param>
    /// <param name="dto">Модель данных для создания элемента корзины</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Созданный элемент корзины</returns>
    Task<DataResult<BasketItemDto>> CreateAsync(Guid userId, CreateBasketItemDto dto, CancellationToken ct = default);

    /// <summary>
    /// Обновить количество товара в элементе корзины
    /// </summary>
    /// <param name="basketItemId">Id элемента корзины</param>
    /// <param name="dto">Модель для обновления количества товара</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Обновлённый элемент корзины</returns>
    Task<DataResult<BasketItemDto>> UpdateQuantityAsync(long basketItemId, UpdateQuantityDto dto, CancellationToken ct = default);

    /// <summary>
    /// Удалить элемент корзины по Id
    /// </summary>
    /// <param name="basketItemId">Id элемента корзины</param>
    /// <param name="ct">Токен отмены операции</param>
    Task<BaseResult> DeleteByIdAsync(long basketItemId, CancellationToken ct = default);

    /// <summary>
    /// Получить элементы корзины по Id пользователя
    /// </summary>
    /// <param name="userId">Id пользователя</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Элементы корзины</returns>
    Task<CollectionResult<BasketItemDto>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
}