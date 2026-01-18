using OrderPaymentSystem.Application.DTOs.Order;
using OrderPaymentSystem.Shared.Result;

namespace OrderPaymentSystem.Application.Interfaces.Services;

/// <summary>
/// Сервис для работы с заказами
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// Создать заказ
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<DataResult<OrderDto>> CreateAsync(Guid userId, CreateOrderDto dto, CancellationToken ct = default);

    /// <summary>
    /// Получить заказ по Id
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<DataResult<OrderDto>> GetByIdAsync(long orderId, CancellationToken ct = default);

    /// <summary>
    /// Обновить статус заказа
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="dto"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<BaseResult> UpdateStatusAsync(long orderId, UpdateOrderStatusDto dto, CancellationToken ct = default);

    /// <summary>
    /// Получить заказы пользователя
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<CollectionResult<OrderDto>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);

    Task<BaseResult> CompleteProcessingAsync(long orderId, long paymentId, CancellationToken ct = default);

    Task<BaseResult> UpdateBulkOrderItemsAsync(long orderId, UpdateBulkOrderItemsDto dto, CancellationToken ct = default);

    Task<BaseResult> ShipOrderAsync(long orderId, CancellationToken ct = default);
}
