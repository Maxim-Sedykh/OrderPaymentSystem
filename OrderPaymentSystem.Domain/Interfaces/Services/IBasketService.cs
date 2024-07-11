using OrderPaymentSystem.Domain.Dto.Basket;
using OrderPaymentSystem.Domain.Dto.Order;
using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Domain.Interfaces.Services
{
    /// <summary>
    /// Сервис для работы с корзиной пользователя
    /// </summary>
    public interface IBasketService
    {
        /// <summary>
        /// Добавление заказа в корзину пользователя
        /// </summary>
        /// <returns></returns>
        Task<CollectionResult<OrderDto>> GetBasketOrdersAsync(long basketId);

        /// <summary>
        /// Получение всех заказов из корзины пользователя
        /// </summary>
        /// <returns></returns>
        Task<CollectionResult<OrderDto>> ClearBasketAsync(long basketId);

        /// <summary>
        /// Получение информации о корзине пользователя
        /// </summary>
        /// <returns></returns>
        Task<BaseResult<BasketDto>> GetBasketByIdAsync(long basketId);
    }
}
