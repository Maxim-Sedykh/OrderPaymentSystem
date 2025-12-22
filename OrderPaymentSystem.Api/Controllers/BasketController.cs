using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OrderPaymentSystem.Domain.Constants;
using OrderPaymentSystem.Domain.Dto.Basket;
using OrderPaymentSystem.Domain.Dto.Order;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces.Services;

namespace OrderPaymentSystem.Api.Controllers;

/// <summary>
/// Контроллер, предназначенный для работы с корзиной заказов пользователя
/// </summary>
/// <remarks>
/// Конструктор контроллера для работы с корзиной заказов
/// </remarks>
/// <param name="basketService">Сервис для работы с корзиной заказов</param>
[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/baskets")]
[ApiController]
public class BasketController(IBasketService basketService) : ControllerBase
{
    private readonly IBasketService _basketService = basketService;

    /// <summary>
    /// Очищение корзины пользователя от всех заказов
    /// </summary>
    /// <param name="basketId"></param>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    /// <remarks>
    /// 
    ///     DELETE
    ///     {
    ///         "id": 1
    ///     }
    ///     
    /// </remarks>
    /// <response code="200">Если корзина была очищена</response>
    /// <response code="400">Если корзина не была очищена</response>
    [HttpDelete("{basketId}/")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderDto>> ClearUserBasket(long basketId, CancellationToken cancellationToken)
    {
        var response = await _basketService.ClearBasketAsync(basketId, cancellationToken);
        if (response.IsSuccess)
        {
            return NoContent();
        }
        if (response.Error.Code == (int)ErrorCodes.BasketNotFound)
        {
            return NotFound(ErrorCodes.BasketNotFound.ToString());
        }

        return BadRequest(response.Error);
    }

    /// <summary>
    /// Получение всех заказов из корзины
    /// </summary>
    /// <param name="basketId"></param>
    /// <param name="cancellationToken">Токен отмены запроса</param>
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
    [HttpGet("{basketId}/orders")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderDto[]>> GetBasketOrdersById(long basketId, CancellationToken cancellationToken)
    {
        var response = await _basketService.GetBasketOrdersAsync(basketId, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }

        return BadRequest(response.Error);
    }

    /// <summary>
    /// Получение информации о корзине пользователя
    /// </summary>
    /// <param name="basketId"></param>
    /// <param name="cancellationToken">Токен отмены запроса</param>
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
    [HttpGet("{basketId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BasketDto>> GetBasketById(long basketId, CancellationToken cancellationToken)
    {
        var response = await _basketService.GetBasketByIdAsync(basketId, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        if (response.Error.Code == (int)ErrorCodes.BasketNotFound)
        {
            return NotFound(ErrorCodes.BasketNotFound.ToString());
        }

        return BadRequest(response.Error);
    }
}
