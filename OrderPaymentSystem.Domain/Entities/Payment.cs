using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces.Entities;
using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Domain.Entities;

/// <summary>
/// Платёж заказа
/// </summary>
public class Payment : IEntityId<long>, IAuditable
{
    /// <summary>
    /// Id платежа
    /// </summary>
    public long Id { get; protected set; }

    /// <summary>
    /// Id заказа
    /// </summary>
    public long OrderId { get; protected set; }

    /// <summary>
    /// Сколько нужно заплатить за заказ
    /// </summary>
    public decimal AmountToPay { get; protected set; }

    /// <summary>
    /// Сколько заплатили
    /// </summary>
    public decimal? AmountPayed { get; protected set; }

    /// <summary>
    /// Сдача
    /// </summary>
    public decimal? CashChange { get; protected set; }

    /// <summary>
    /// Способ оплаты
    /// </summary>
    public PaymentMethod PaymentMethod { get; protected set; }

    /// <summary>
    /// Текущий статус платежа
    /// </summary>
    public PaymentStatus Status { get; protected set; }

    /// <inheritdoc/>
    public DateTime CreatedAt { get; protected set; }

    /// <inheritdoc/>
    public DateTime? UpdatedAt { get; protected set; }

    /// <summary>
    /// Заказ
    /// </summary>
    public Order Order { get; protected set; }

    protected Payment() { }

    /// <summary>
    /// Создать платёж
    /// </summary>
    /// <param name="orderId">Id заказа</param>
    /// <param name="amountToPay">Количество денег которое нужно заплатить</param>
    /// <param name="amountPayed">Фактическое количество денег которое было заплачено</param>
    /// <param name="cashChange">Сдача</param>
    /// <param name="method">Метод платежа</param>
    /// <returns>Результат создания платежа</returns>
    public static DataResult<Payment> Create(long orderId, decimal amountToPay, decimal amountPayed, decimal cashChange, PaymentMethod method)
    {
        if (amountToPay <= 0) return DataResult<Payment>.Failure(8001, "Amount to pay must be positive.");

        return DataResult<Payment>.Success(new Payment
        {
            Id = default,
            OrderId = orderId,
            AmountToPay = amountToPay,
            AmountPayed = amountPayed,
            CashChange = cashChange,
            PaymentMethod = method,
            Status = PaymentStatus.Pending
        });
    }
}
