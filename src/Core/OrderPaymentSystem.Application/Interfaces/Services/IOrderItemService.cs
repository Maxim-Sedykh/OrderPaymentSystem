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
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<DataResult<OrderItemDto>> CreateAsync(CreateOrderItemDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить количество товара в элементе заказа
    /// </summary>
    /// <param name="orderItemId"></param>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<DataResult<OrderItemDto>> UpdateQuantityAsync(long orderItemId, UpdateQuantityDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить элемент заказа по Id
    /// </summary>
    /// <param name="orderItemId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult> DeleteByIdAsync(long orderItemId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить элементы заказа по его идентификатору
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CollectionResult<OrderItemDto>> GetByOrderIdAsync(long orderId, CancellationToken cancellationToken = default);
}
