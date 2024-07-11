using OrderPaymentSystem.Domain.Dto.Order;
using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Domain.Interfaces.Services
{
    /// <summary>
    /// Сервис для работы с заказами
    /// </summary>
    public interface IOrderService
    {
        /// <summary>
        /// Получение всех заказов всех пользователей
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<CollectionResult<OrderDto>> GetAllOrdersAsync();

        /// <summary>
        /// Получение заказа по идентификатору
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<BaseResult<OrderDto>> GetOrderByIdAsync(long id);

        /// <summary>
        /// Удаление заказа по идентификатору
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<BaseResult<OrderDto>> DeleteOrderByIdAsync(long id);

        /// <summary>
        /// Обновление заказа
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<BaseResult<OrderDto>> UpdateOrderAsync(UpdateOrderDto dto);

        /// <summary>
        /// Создание заказа и добавление его в корзину
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<BaseResult<OrderDto>> CreateOrderAsync(CreateOrderDto dto);
    }
}
