using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderPaymentSystem.Domain.Constants;
using OrderPaymentSystem.Domain.Dto.Order;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Api.Controllers
{
    /// <summary>
    /// Контроллер, предназначенный для работы с корзиной заказов пользователя
    /// </summary>
    //[Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketService _basketService;

        public BasketController(IBasketService basketService)
        {
            _basketService = basketService;
        }

        /// <summary>
        /// Очищение корзины пользователя от всех заказов
        /// </summary>
        /// <param name="basketId"></param>
        /// <remarks>
        /// Request for clear basket
        /// 
        ///     DELETE
        ///     {
        ///         "id": 1
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Если корзина была очищена</response>
        /// <response code="400">Если корзина не была очищена</response>
        [HttpDelete(RouteConstants.ClearUserBasketById)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<OrderDto>>> ClearUserBasket(long basketId)
        {
            var response = await _basketService.ClearBasketAsync(basketId);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        /// <summary>
        /// Получение всех заказов из корзины
        /// </summary>
        /// <param name="basketId"></param>
        /// <remarks>
        /// Request for getting user basket orders
        /// 
        ///     GET
        ///     {
        ///         "id": 1
        ///     }
        ///  
        /// </remarks>
        /// <response code="200">Если заказы из корзины были получены</response>
        /// <response code="400">Если заказы из корзины не были получены</response>
        [HttpGet(RouteConstants.GetUserBasketOrdersByBasketId)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CollectionResult<OrderDto>>> GetUserBasketOrders(long basketId)
        {
            var response = await _basketService.GetBasketOrdersAsync(basketId);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        /// <summary>
        /// Получение информации о корзине пользователя
        /// </summary>
        /// <param name="basketId"></param>
        /// <remarks>
        /// Request for getting user basket info
        /// 
        ///     GET
        ///     {
        ///         "id": 1
        ///     }
        ///  
        /// </remarks>
        /// <response code="200">Если корзина пользователя была получена</response>
        /// <response code="400">Если корзина пользователя не была получена</response>
        [HttpGet(RouteConstants.GetBasketById)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CollectionResult<OrderDto>>> GetBasketById(long basketId)
        {
            var response = await _basketService.GetBasketByIdAsync(basketId);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
