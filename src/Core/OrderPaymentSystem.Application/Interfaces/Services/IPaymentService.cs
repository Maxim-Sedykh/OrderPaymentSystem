using OrderPaymentSystem.Application.DTOs.Payment;
using OrderPaymentSystem.Shared.Result;

namespace OrderPaymentSystem.Application.Interfaces.Services;

/// <summary>
/// Интерфейс сервиса для работы с платежами
/// </summary>
public interface IPaymentService
{
    /// <summary>
    /// Создать платёж
    /// </summary>
    /// <param name="dto">Модель данных для создания платежа</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Созданный платёж</returns>
    Task<DataResult<PaymentDto>> CreateAsync(CreatePaymentDto dto, CancellationToken ct = default);

    /// <summary>
    /// Получить платёж по Id
    /// </summary>
    /// <param name="paymentId">Id платежа</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Платёж</returns>
    Task<DataResult<PaymentDto>> GetByIdAsync(long paymentId, CancellationToken ct = default);

    /// <summary>
    /// Получить платёж по идентификатору заказа привязанному к платежу
    /// </summary>
    /// <param name="orderId">Id заказа</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Платежи по заказу</returns>
    Task<CollectionResult<PaymentDto>> GetByOrderIdAsync(long orderId, CancellationToken ct = default);

    /// <summary>
    /// Завершить платёж
    /// </summary>
    /// <param name="paymentId">Id платежа</param>
    /// <param name="dto">Модель для завершения заказа</param>
    /// <param name="ct">Токен отмены операции</param>
    Task<BaseResult> CompletePaymentAsync(long paymentId, CompletePaymentDto dto, CancellationToken ct = default);
}

