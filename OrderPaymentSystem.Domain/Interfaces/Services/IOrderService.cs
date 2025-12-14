using OrderPaymentSystem.Domain.Dto.Order;
using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Domain.Interfaces.Services;

/// <summary>
/// Сервис для работы с заказами
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// Получение всех заказов всех пользователей
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<CollectionResult<OrderDto>> GetAllOrdersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение заказа по идентификатору
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<DataResult<OrderDto>> GetOrderByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаление заказа по идентификатору
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<BaseResult> DeleteOrderByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновление заказа
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<DataResult<OrderDto>> UpdateOrderAsync(long id, UpdateOrderDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создание заказа и добавление его в корзину
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<BaseResult> CreateOrderAsync(CreateOrderDto dto, CancellationToken cancellationToken = default);
}
