using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderPaymentSystem.Application.Validations.FluentValidations.Order;
using OrderPaymentSystem.Domain.Constants;
using OrderPaymentSystem.Domain.Dto.Order;
using OrderPaymentSystem.Domain.Dto.Product;
using OrderPaymentSystem.Domain.Interfaces.Services;

namespace OrderPaymentSystem.Api.Controllers;

/// <summary>
/// Контроллер для работы с товарами
/// </summary>
[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly UpdateOrderValidation _updateOrderValidator;
    private readonly CreateOrderValidation _createOrderValidator;

    /// <summary>
    /// Конструктор для контроллера, выполняющий работу с заказами
    /// </summary>
    /// <param name="orderService">Сервис заказов</param>
    /// <param name="updateOrderValidator">Валидатор обновления заказа</param>
    /// <param name="createOrderValidator">Валидатор создания заказа</param>
    public OrderController(IOrderService orderService, UpdateOrderValidation updateOrderValidator,
        CreateOrderValidation createOrderValidator)
    {
        _orderService = orderService;
        _updateOrderValidator = updateOrderValidator;
        _createOrderValidator = createOrderValidator;
    }

    /// <summary>
    /// Получение заказа по ID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    /// <remarks>
    /// Request for getting order
    /// 
    ///     GET
    ///     {
    ///         "id": 1
    ///     }
    ///     
    /// </remarks>
    /// <response code="200">Если заказ был получен</response>
    /// <response code="400">Если заказ не был получен</response>
    [HttpGet(RouteConstants.GetOrderById)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderDto>> GetOrderById(long id, CancellationToken cancellationToken)
    {
        var response = await _orderService.GetOrderByIdAsync(id, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
    }

    /// <summary>
    /// Получение всех заказов
    /// </summary>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    /// <remarks>
    /// Request for getting all orders
    /// </remarks>
    /// <response code="200">Если заказы были получены</response>
    /// <response code="400">Если заказы не были получены</response>
    [HttpGet(RouteConstants.GetOrders)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderDto>> GetAllOrders(CancellationToken cancellationToken)
    {
        var response = await _orderService.GetAllOrdersAsync(cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
    }

    /// <summary>
    /// Удаление заказа
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    /// <remarks>
    /// Request for delete order
    /// 
    ///     DELETE
    ///     {
    ///         "id": 1
    ///     }
    ///     
    /// </remarks>
    /// <response code="200">Если заказ удалился</response>
    /// <response code="400">Если заказ не был удалён</response>
    [HttpDelete(RouteConstants.DeleteOrderById)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> DeleteOrder(long id, CancellationToken cancellationToken)
    {
        var response = await _orderService.DeleteOrderByIdAsync(id, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
    }

    /// <summary>
    /// Создание заказа
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    /// <remarks>
    /// Request for create order
    /// 
    ///     POST
    ///     {
    ///         "userid": 2,
    ///         "productid": 1,
    ///         "productcound": 5
    ///     }
    ///     
    /// </remarks>
    /// <response code="200">Если заказ создался</response>
    /// <response code="400">Если заказ не был создан</response>
    [HttpPost(RouteConstants.CreateOrder)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> CreateOrder(CreateOrderDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _createOrderValidator.ValidateAsync(dto, cancellationToken);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var response = await _orderService.CreateOrderAsync(dto, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
    }

    /// <summary>
    /// Обновление заказа
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    /// <remarks>
    /// Request for update order
    /// 
    ///     PUT
    ///     {
    ///         "id": 1
    ///         "productid": 1,
    ///         "ProductCount": 3
    ///     }
    ///     
    /// </remarks>
    /// <response code="200">Если заказ обновился</response>
    /// <response code="400">Если заказ не был обновлён</response>
    [HttpPut(RouteConstants.UpdateOrder)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> UpdateOrder(UpdateOrderDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _updateOrderValidator.ValidateAsync(dto, cancellationToken);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var response = await _orderService.UpdateOrderAsync(dto, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
    }
}
