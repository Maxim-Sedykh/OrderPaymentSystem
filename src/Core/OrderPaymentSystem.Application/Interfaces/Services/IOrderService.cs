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
    /// <param name="userId">Id текущего авторизованного пользователя</param>
    /// <param name="dto">Модель для создания заказа</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Id созданного товара</returns>
    Task<DataResult<long>> CreateAsync(Guid userId, CreateOrderDto dto, CancellationToken ct = default);

    /// <summary>
    /// Получить заказ по Id
    /// </summary>
    /// <param name="orderId">Id заказа</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Заказ</returns>
    Task<DataResult<OrderDto>> GetByIdAsync(long orderId, CancellationToken ct = default);

    /// <summary>
    /// Обновить статус заказа
    /// </summary>
    /// <param name="orderId">Id заказа</param>
    /// <param name="dto">Модель для обновления статуса</param>
    /// <param name="ct">Токен отмены операции</param>
    Task<BaseResult> UpdateStatusAsync(long orderId, UpdateOrderStatusDto dto, CancellationToken ct = default);

    /// <summary>
    /// Получить заказы пользователя
    /// </summary>
    /// <param name="userId">Id текущего авторизованного пользователя</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Заказы пользователя</returns>
    Task<CollectionResult<OrderDto>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);

    /// <summary>
    /// Завершить обработку заказа
    /// </summary>
    /// <param name="orderId">Id заказа</param>
    /// <param name="paymentId">Id платежа</param>
    /// <param name="ct">Токен отмены операции</param>
    Task<BaseResult> CompleteProcessingAsync(long orderId, long paymentId, CancellationToken ct = default);

    /// <summary>
    /// Массовое обновление элементов заказа
    /// </summary>
    /// <param name="orderId">Id заказа</param>
    /// <param name="dto">Модель данных для массового обновления элементов заказа</param>
    /// <param name="ct">Токен отмены операции</param>
    Task<BaseResult> UpdateBulkOrderItemsAsync(long orderId, UpdateBulkOrderItemsDto dto, CancellationToken ct = default);

    /// <summary>
    /// Поменять статус заказа на Shipped (Доставлен)
    /// </summary>
    /// <param name="orderId">Id заказа</param>
    /// <param name="ct">Токен отмены операции</param>
    Task<BaseResult> ShipOrderAsync(long orderId, CancellationToken ct = default);
}
