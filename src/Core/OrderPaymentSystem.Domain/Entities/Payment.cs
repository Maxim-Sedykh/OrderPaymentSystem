using OrderPaymentSystem.Domain.Abstract;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Entities;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Shared.Exceptions;

namespace OrderPaymentSystem.Domain.Entities;

/// <summary>
/// Платёж заказа
/// </summary>
public class Payment : BaseEntity<long>, IAuditable
{
    /// <summary>
    /// Id заказа
    /// </summary>
    public long OrderId { get; private set; }

    /// <summary>
    /// Сколько нужно заплатить за заказ
    /// </summary>
    public decimal AmountToPay { get; private set; }

    /// <summary>
    /// Сколько заплатили
    /// </summary>
    public decimal? AmountPaid { get; private set; }

    /// <summary>
    /// Сдача
    /// </summary>
    public decimal? CashChange { get; private set; }

    /// <summary>
    /// Способ оплаты
    /// </summary>
    public PaymentMethod Method { get; private set; }

    /// <summary>
    /// Текущий статус платежа
    /// </summary>
    public PaymentStatus Status { get; private set; }

    /// <inheritdoc/>
    public DateTime CreatedAt { get; private set; }

    /// <inheritdoc/>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Заказ
    /// </summary>
    public Order Order { get; protected set; }

    private Payment() { }

    private Payment(long orderId,
        decimal amountPayed,
        decimal amountToPay,
        PaymentMethod method,
        PaymentStatus status) 
    {
        OrderId = orderId;
        AmountToPay = amountToPay;
        AmountPaid = amountPayed;
        Method = method;
        Status = status;
    }

    private Payment(
        long id,
        long orderId,
        decimal amountPayed,
        decimal amountToPay,
        PaymentMethod method,
        PaymentStatus status) 
    {
        Id = id;
        OrderId = orderId;
        AmountToPay = amountToPay;
        AmountPaid = amountPayed;
        Method = method;
        Status = status;
    }

    internal static Payment CreateExisting(
        long id,
        long orderId,
        decimal amountPaid,
        decimal amountToPay,
        PaymentMethod method,
        PaymentStatus status)
    {
        ValidateCreate(orderId, amountPaid, amountToPay);

        return new Payment(id, orderId, amountPaid, amountToPay, method, status);
    }

    /// <summary>
    /// Создать платёж
    /// </summary>
    /// <param name="orderId">Id заказа</param>
    /// <param name="amountToPay">Количество денег которое нужно заплатить</param>
    /// <param name="method">Метод платежа</param>
    /// <returns>Созданный платёж</returns>
    public static Payment Create(
        long orderId,
        decimal amountPaid,
        decimal amountToPay,
        PaymentMethod method)
    {
        ValidateCreate(orderId, amountPaid, amountToPay);

        return new Payment(orderId, amountPaid, amountToPay, method, PaymentStatus.Pending);
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

        AmountPaid = amountPaid;
        CashChange = cashChange;
        Status = PaymentStatus.Succeeded;
    }

    private static void ValidateCreate(long orderId, decimal amoundPaid, decimal amountToPay)
    {
        if (orderId < 0)
            throw new BusinessException(DomainErrors.Validation.InvalidFormat(nameof(orderId)));

        if (amoundPaid < 0)
            throw new BusinessException(DomainErrors.Payment.AmountPositive());

        if (amountToPay <= 0)
            throw new BusinessException(DomainErrors.Payment.AmountPositive());
    }
}
