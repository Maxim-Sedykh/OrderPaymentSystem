using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using OrderPaymentSystem.Application.DTOs.Order;
using OrderPaymentSystem.Application.DTOs.Payment;
using OrderPaymentSystem.Application.Interfaces.Services;

namespace OrderPaymentSystem.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost]
    public async Task<ActionResult> Create(CreatePaymentDto dto, CancellationToken cancellationToken)
    {
        var response = await _paymentService.CreateAsync(dto, cancellationToken);
        if (response.IsSuccess)
        {
            return NoContent();
        }
        return BadRequest(response.Error);
    }

    [HttpGet("/{paymentId}")]
    public async Task<ActionResult<PaymentDto>> GetById(long paymentId, CancellationToken cancellationToken)
    {
        var response = await _paymentService.GetByIdAsync(paymentId, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
    }

    [HttpGet("/{orderId}")]
    public async Task<ActionResult<OrderDto>> GetByOrderId(long orderId, CancellationToken cancellationToken)
    {
        var response = await _paymentService.GetByOrderIdAsync(orderId, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Complete(long paymentId, CompletePaymentDto dto, CancellationToken cancellationToken)
    {
        var response = await _paymentService.CompletePaymentAsync(paymentId, dto, cancellationToken);
        if (response.IsSuccess)
        {
            return NoContent();
        }
        return BadRequest(response.Error);
    }
}
