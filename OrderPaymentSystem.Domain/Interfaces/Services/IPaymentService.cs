using OrderPaymentSystem.Domain.Dto.Payment;
using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Domain.Interfaces.Services;

/// <summary>
/// Интерфейс сервиса для работы с платежами
/// </summary>
public interface IPaymentService
{
    /// <summary>
    /// Создать платёж
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult> CreateAsync(CreatePaymentDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить платёж по Id
    /// </summary>
    /// <param name="paymentId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<DataResult<PaymentDto>> GetByIdAsync(long paymentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить платёж по идентификатору заказа привязанному к платежу
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CollectionResult<PaymentDto>> GetByOrderIdAsync(long orderId, CancellationToken cancellationToken = default);

    Task<BaseResult> CompletePaymentAsync(long paymentId, decimal amountPaid, decimal cashChange, CancellationToken cancellationToken = default)
}

