using OrderPaymentSystem.Domain.Dto.Order;
using OrderPaymentSystem.Domain.Dto.Payment;
using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Domain.Interfaces.Services
{
    public interface IPaymentService
    {
        /// <summary>
        /// Получение платежей пользователя по его идентификатору
        /// </summary>
        /// <returns></returns>
        Task<CollectionResult<PaymentDto>> GetUserPaymentsAsync(long userId);

        /// <summary>
        /// Получение всех заказов платежа по идентификатору
        /// </summary>
        /// <returns></returns>
        Task<CollectionResult<OrderDto>> GetPaymentOrdersAsync(long id);

        /// <summary>
        /// Получение платежа по идентификатору
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<BaseResult<PaymentDto>> GetPaymentByIdAsync(long id);

        /// <summary>
        /// Добавление платежа
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<BaseResult<PaymentDto>> CreatePaymentAsync(CreatePaymentDto dto);

        /// <summary>
        /// Удаление платеэа по идентификатору
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<BaseResult<PaymentDto>> DeletePaymentAsync(long id);

        /// <summary>
        /// Обновление платежа
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<BaseResult<PaymentDto>> UpdatePaymentAsync(UpdatePaymentDto dto);
    }
}
