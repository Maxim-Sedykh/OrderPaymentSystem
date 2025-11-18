using OrderPaymentSystem.Domain.Dto.Order;
using OrderPaymentSystem.Domain.Dto.Payment;
using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Domain.Interfaces.Services;

/// <summary>
/// Интерфейс сервиса для работы с платежами
/// </summary>
public interface IPaymentService
{
    /// <summary>
    /// Получение платежей пользователя по его идентификатору
    /// </summary>
    /// <returns></returns>
    Task<CollectionResult<PaymentDto>> GetUserPaymentsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение всех заказов платежа по идентификатору
    /// </summary>
    /// <returns></returns>
    Task<CollectionResult<OrderDto>> GetPaymentOrdersAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение платежа по идентификатору
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<DataResult <PaymentDto>> GetPaymentByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавление платежа
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<DataResult<PaymentDto>> CreatePaymentAsync(CreatePaymentDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаление платеэа по идентификатору
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<DataResult<PaymentDto>> DeletePaymentAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновление платежа
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<DataResult<PaymentDto>> UpdatePaymentAsync(UpdatePaymentDto dto, CancellationToken cancellationToken = default);
}
