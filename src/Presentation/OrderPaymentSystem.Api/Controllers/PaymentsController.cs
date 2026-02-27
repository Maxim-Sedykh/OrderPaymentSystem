using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderPaymentSystem.Application.DTOs.Payment;
using OrderPaymentSystem.Application.Interfaces.Services;

namespace OrderPaymentSystem.Api.Controllers;

/// <summary>
/// Контроллер для работы с платежами
/// </summary>
[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="paymentService">Сервис для работы с платежами</param>
    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    /// <summary>
    /// Создать платёж
    /// </summary>
    /// <param name="dto">Модель создания</param>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    /// <returns>Созданный платёж</returns>
    [HttpPost]
    public async Task<ActionResult<PaymentDto>> Create(CreatePaymentDto dto, CancellationToken cancellationToken)
    {
        var response = await _paymentService.CreateAsync(dto, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
    }

    /// <summary>
    /// Получить платёж по Id
    /// </summary>
    /// <param name="id">Id платежа</param>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    /// <returns>Платёж в виде <see cref="PaymentDto"/></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<PaymentDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var response = await _paymentService.GetByIdAsync(id, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return NotFound(response.Error);
    }

    /// <summary>
    /// Получить платежи заказа
    /// </summary>
    /// <param name="orderId">Id заказа</param>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    /// <returns>Платежа по заказу</returns>
    [HttpGet("orders/{orderId}")]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetByOrderId(long orderId, CancellationToken cancellationToken)
    {
        var response = await _paymentService.GetByOrderIdAsync(orderId, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
    }

    /// <summary>
    /// Оплатить платёж
    /// </summary>
    /// <param name="id">Id платежа</param>
    /// <param name="dto">Модель данных оплаты платежа</param>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    [HttpPost("{id}/complete")]
    public async Task<ActionResult> Complete(long id, CompletePaymentDto dto, CancellationToken cancellationToken)
    {
        var response = await _paymentService.CompletePaymentAsync(id, dto, cancellationToken);
        if (response.IsSuccess)
        {
            return NoContent();
        }
        return BadRequest(response.Error);
    }
}
