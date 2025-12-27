using OrderPaymentSystem.Domain.Dto;
using OrderPaymentSystem.Domain.Dto.Basket;
using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Domain.Interfaces.Services;

/// <summary>
/// Интерфейс сервиса для работы с элементами корзины пользователя
/// </summary>
public interface IBasketItemService
{
    /// <summary>
    /// Создать элемент корзины пользователя
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<DataResult<BasketItemDto>> CreateAsync(Guid userId, CreateBasketItemDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить количество товара в элементе корзины
    /// </summary>
    /// <param name="basketItemId"></param>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<DataResult<BasketItemDto>> UpdateQuantityAsync(long basketItemId, UpdateQuantityDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить элемент корзины по Id
    /// </summary>
    /// <param name="basketItemId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult> DeleteByIdAsync(long basketItemId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить элементы корзины по Id пользователя
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CollectionResult<BasketItemDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}