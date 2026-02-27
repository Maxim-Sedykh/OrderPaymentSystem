namespace OrderPaymentSystem.Domain.Enum;

/// <summary>
/// Метод платежа
/// </summary>
public enum PaymentMethod
{
    /// <summary>
    /// Кредитная карта
    /// </summary>
    CreditCard = 0,

    /// <summary>
    /// Дебитовая карта
    /// </summary>
    DebitCard = 1,

    /// <summary>
    /// Через сервис PayPal
    /// </summary>
    PayPal = 2,

    /// <summary>
    /// Через ApplePay
    /// </summary>
    ApplePay = 3,

    /// <summary>
    /// GooglePay
    /// </summary>
    GooglePay = 4,
    
    /// <summary>
    /// Наличкой при доставке
    /// </summary>
    CashOnDelivery = 7,
    
    /// <summary>
    /// Крипто-кошелёк
    /// </summary>
    Cryptocurrency = 8,

    /// <summary>
    /// Наличкой при заказе
    /// </summary>
    Cash = 10
}
