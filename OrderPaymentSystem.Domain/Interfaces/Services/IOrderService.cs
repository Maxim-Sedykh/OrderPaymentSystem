using OrderPaymentSystem.Domain.Dto.Order;
using OrderPaymentSystem.Domain.Dto.OrderItem;
using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Domain.Interfaces.Services;

/// <summary>
/// Сервис для работы с заказами
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// Создать заказ
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<DataResult<OrderDto>> CreateAsync(Guid userId, CreateOrderDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить заказ по Id
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<DataResult<OrderDto>> GetByIdAsync(long orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить статус заказа
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult> UpdateStatusAsync(long orderId, UpdateOrderStatusDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить заказы пользователя
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CollectionResult<OrderDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<BaseResult> CompleteProcessingAsync(long orderId, long paymentId, CancellationToken cancellationToken = default);

    Task<BaseResult> UpdateBulkOrderItemsAsync(long orderId, List<UpdateOrderItemDto> adjustments, CancellationToken cancellationToken = default);

    Task<BaseResult> ShipOrderAsync(long orderId, CancellationToken cancellationToken = default);
}
