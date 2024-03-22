using OrderPaymentSystem.Domain.Dto.Order;
using OrderPaymentSystem.Domain.Dto.Product;
using OrderPaymentSystem.Domain.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        Task<CollectionResult<OrderDto>> GetUserBasketOrdersAsync();

        /// <summary>
        /// Получение всех заказов из корзины пользователя
        /// </summary>
        /// <returns></returns>
        Task<CollectionResult<OrderDto>> ClearUserBasketAsync();
    }
}
