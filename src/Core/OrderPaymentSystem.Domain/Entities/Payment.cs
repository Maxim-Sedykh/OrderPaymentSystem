using OrderPaymentSystem.Domain.Abstract.Interfaces.Entities;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Errors;
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
    public PaymentMethod Method { get; protected set; }

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
        decimal amountPayed,
        decimal amountToPay,
        PaymentMethod method)
    {
        if (amountPayed <= 0)
            throw new BusinessException(DomainErrors.Payment.AmountPositive());

        if (amountToPay <= 0)
            throw new BusinessException(DomainErrors.Payment.AmountPositive());

        return new Payment
        {
            Id = default,
            OrderId = orderId,
            AmountToPay = amountToPay,
            AmountPayed = amountPayed,
            Method = method,
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
            throw new BusinessException(DomainErrors.Payment.InvalidStatus(Status.ToString(), PaymentStatus.Succeeded.ToString()));
        if (amountPaid <= 0)
            throw new BusinessException(DomainErrors.Payment.AmountPositive());
        if (amountPaid < AmountToPay)
            throw new BusinessException(DomainErrors.Payment.NotEnoughAmount(amountPaid, AmountToPay));
        if (amountPaid - AmountToPay != cashChange)
            throw new BusinessException(DomainErrors.Payment.CashChangeMismatch());

        AmountPayed = amountPaid;
        CashChange = cashChange;
        Status = PaymentStatus.Succeeded;
    }
}
