using OrderPaymentSystem.Application.DTOs;
using OrderPaymentSystem.Application.DTOs.OrderItem;
using OrderPaymentSystem.Shared.Result;

namespace OrderPaymentSystem.Application.Interfaces.Services;

/// <summary>
/// Интерфейс сервиса для работы с элементами заказа
/// </summary>
public interface IOrderItemService
{
    /// <summary>
    /// Создать элемент заказа
    /// </summary>
    /// <param name="orderId">Id заказа</param>
    /// <param name="dto">Модель для создания элемента</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Созданный элемент заказа</returns>
    Task<DataResult<OrderItemDto>> CreateAsync(long orderId, CreateOrderItemDto dto, CancellationToken ct = default);

    /// <summary>
    /// Обновить количество товара в элементе заказа
    /// </summary>
    /// <param name="orderItemId">Id элемента заказа</param>
    /// <param name="dto">Модель для обновления количества товара</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Обновлённый элемент заказа</returns>
    Task<DataResult<OrderItemDto>> UpdateQuantityAsync(long orderItemId, UpdateQuantityDto dto, CancellationToken ct = default);

    /// <summary>
    /// Удалить элемент заказа по Id
    /// </summary>
    /// <param name="orderItemId">Id элемента заказа</param>
    /// <param name="ct">Токен отмены операции</param>
    Task<BaseResult> DeleteByIdAsync(long orderItemId, CancellationToken ct = default);

    /// <summary>
    /// Получить элементы заказа по его идентификатору
    /// </summary>
    /// <param name="orderId">Id заказа</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Коллекция элементов заказа</returns>
    Task<CollectionResult<OrderItemDto>> GetByOrderIdAsync(long orderId, CancellationToken ct = default);
}
