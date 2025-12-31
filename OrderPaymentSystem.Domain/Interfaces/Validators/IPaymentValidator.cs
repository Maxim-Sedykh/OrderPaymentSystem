using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Domain.Interfaces.Validators
{
    public interface IPaymentValidator
    {
        /// <summary>
        /// Валидировать создание платежа
        /// </summary>
        /// <returns>Результат валидации</returns>
        BaseResult ValidateCreatingPayment(bool orderExists, bool paymentExists, long orderId);
    }
}
