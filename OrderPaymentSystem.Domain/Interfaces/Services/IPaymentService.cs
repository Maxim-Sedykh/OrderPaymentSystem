using OrderPaymentSystem.Domain.Dto.Order;
using OrderPaymentSystem.Domain.Dto.Payment;
using OrderPaymentSystem.Domain.Dto.Product;
using OrderPaymentSystem.Domain.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        Task<CollectionResult<OrderDto>> GetPaymentOrdersAsync(long paymentId);

        /// <summary>
        /// Получение платежа по идентификатору
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<BaseResult<PaymentDto>> GetPaymentByIdAsync(int id);

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
        Task<BaseResult<PaymentDto>> DeletePaymentAsync(int id);

        /// <summary>
        /// Обновление платежа
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<BaseResult<PaymentDto>> UpdatePaymentAsync(UpdatePaymentDto dto);
    }
}
