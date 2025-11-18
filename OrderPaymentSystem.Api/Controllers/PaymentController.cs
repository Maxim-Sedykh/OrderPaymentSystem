using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderPaymentSystem.Application.Validations.FluentValidations.Payment;
using OrderPaymentSystem.Domain.Constants;
using OrderPaymentSystem.Domain.Dto.Order;
using OrderPaymentSystem.Domain.Dto.Payment;
using OrderPaymentSystem.Domain.Dto.Product;
using OrderPaymentSystem.Domain.Interfaces.Services;

namespace OrderPaymentSystem.Api.Controllers;

/// <summary>
/// Контроллер для работы с платежами
/// </summary>
[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly UpdatePaymentValidation _updatePaymentValidator;
    private readonly CreatePaymentValidator _createPaymentValidator;

    /// <summary>
    /// Конструктор контроллера для работы с платежами
    /// </summary>
    /// <param name="paymentService"></param>
    /// <param name="updatePaymentValidator"></param>
    /// <param name="createPaymentValidator"></param>
    public PaymentController(IPaymentService paymentService, UpdatePaymentValidation updatePaymentValidator,
        CreatePaymentValidator createPaymentValidator)
    {
        _paymentService = paymentService;
        _updatePaymentValidator = updatePaymentValidator;
        _createPaymentValidator = createPaymentValidator;
    }

    /// <summary>
    /// Получение платежа по ID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    /// <remarks>
    /// Request for getting payment
    /// 
    ///     GET
    ///     {
    ///         "id": 1
    ///     }
    ///     
    /// </remarks>
    /// <response code="200">Если платеж был получен</response>
    /// <response code="400">Если платеж не был получен</response>
    [HttpGet(RouteConstants.GetPaymentById)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaymentDto>> GetPayment(long id, CancellationToken cancellationToken)
    {
        var response = await _paymentService.GetPaymentByIdAsync(id, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
    }

    /// <summary>
    /// Получение всех платежей пользователя
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    ///<remarks>
    /// Request for getting user payments
    /// 
    ///     DELETE
    ///     {
    ///         "userid": 1
    ///     }
    ///     
    /// </remarks>
    /// <response code="200">Если платежи пользователя были получены</response>
    /// <response code="400">Если платежи пользователя не были получены</response>
    [HttpGet(RouteConstants.GetUserPaymentsByUserId)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaymentDto>> GetUserPayments(Guid userId, CancellationToken cancellationToken)
    {
        var response = await _paymentService.GetUserPaymentsAsync(userId, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    /// <summary>
    /// Удаление платежа по идентификатору
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    /// <remarks>
    /// Request for delete payment
    /// 
    ///     DELETE
    ///     {
    ///         "id": 1
    ///     }
    ///     
    /// </remarks>
    /// <response code="200">Если платёж удалился</response>
    /// <response code="400">Если платёж не был удалён</response>
    [HttpDelete(RouteConstants.DeletePaymentById)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaymentDto>> DeletePayment(long id, CancellationToken cancellationToken)
    {
        var response = await _paymentService.DeletePaymentAsync(id, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
    }

    /// <summary>
    /// Создание платежа
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    /// <remarks>
    /// Request for create payment
    ///     
    ///     POST
    ///     {
    ///         "basketId": 0,
    ///         "amountOfPayment": 0,
    ///         "paymentMethod": 0,
    ///         "street": "string",
    ///         "city": "string",
    ///         "country": "string",
    ///         "zipcode": "string"
    ///     }
    ///     
    /// </remarks>
    /// <response code="200">Если платеж создался</response>
    /// <response code="400">Если платеж не был создан</response>
    [HttpPost(RouteConstants.CreatePayment)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaymentDto>> CreatePayment(CreatePaymentDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _createPaymentValidator.ValidateAsync(dto, cancellationToken);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var response = await _paymentService.CreatePaymentAsync(dto, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
    }

    /// <summary>
    /// Обновление платежа
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    /// <remarks>
    /// Request for update payment
    /// 
    ///     PUT
    ///     {
    ///         "id": 1
    ///         "amountofpayment": 4000,
    ///         "paymentmethod": 2
    ///     }
    ///     
    /// </remarks>
    /// <response code="200">Если платёж обновился</response>
    /// <response code="400">Если платёж не был обновлён</response>
    [HttpPut(RouteConstants.UpdatePayment)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> UpdatePayment(UpdatePaymentDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _updatePaymentValidator.ValidateAsync(dto, cancellationToken);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var response = await _paymentService.UpdatePaymentAsync(dto, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
    }

    /// <summary>
    /// Получение заказов определённого платежа по идентификатору
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    /// <remarks>
    /// Request for getting payment orders
    /// 
    ///     PUT
    ///     {
    ///         "id": 1
    ///     }
    ///     
    /// </remarks>
    /// <response code="200">Если заказы платежа были получены</response>
    /// <response code="400">Если заказы платежа не были получены</response>
    [HttpGet(RouteConstants.GetPaymentOrdersByPaymentId)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderDto>> GetPaymentOrders(long id, CancellationToken cancellationToken)
    {
        var response = await _paymentService.GetPaymentOrdersAsync(id, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
    }
}
