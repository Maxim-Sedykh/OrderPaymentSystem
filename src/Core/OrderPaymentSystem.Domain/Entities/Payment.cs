using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces.Entities;
using OrderPaymentSystem.Shared.Exceptions;

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
    /// <param name="method">Метод платежа</param>
    /// <returns>Созданный платёж</returns>
    public static Payment Create(
        long orderId,
        decimal amountToPay,
        PaymentMethod method)
    {
        if (amountToPay <= 0)
            throw new BusinessException(8001, "Amount to pay must be positive.");

        return new Payment
        {
            Id = default,
            OrderId = orderId,
            AmountToPay = amountToPay,
            PaymentMethod = method,
            Status = PaymentStatus.Pending
        };
    }

    /// <summary>
    /// Обработать платеж.
    /// </summary>
    /// <param name="amountPaid">Фактическая сумма, которую оплатил клиент.</param>
    /// <param name="cashChange">Сдача, если платеж наличными.</param>
    public void ProcessPayment(decimal amountPaid, decimal cashChange)
    {
        if (Status != PaymentStatus.Pending)
            throw new BusinessException(666, $"Payment is already in {Status} status.");
        if (amountPaid <= 0)
            throw new BusinessException(666, "Amount paid must be positive.");
        if (amountPaid < AmountToPay)
            throw new BusinessException(666, $"Amount paid {amountPaid} is less than amount to pay {AmountToPay}.");
        if (amountPaid - AmountToPay != cashChange)
            throw new BusinessException(666, "Cash change does not match calculation.");

        AmountPayed = amountPaid;
        CashChange = cashChange;
        Status = PaymentStatus.Succeeded;
    }
}
