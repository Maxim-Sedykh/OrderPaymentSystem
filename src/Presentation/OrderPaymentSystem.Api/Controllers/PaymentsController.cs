using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderPaymentSystem.Application.DTOs.Payment;
using OrderPaymentSystem.Application.Interfaces.Services;

namespace OrderPaymentSystem.Api.Controllers;

[Authorize]
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
    public async Task<ActionResult<PaymentDto>> Create(CreatePaymentDto dto, CancellationToken cancellationToken)
    {
        var response = await _paymentService.CreateAsync(dto, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
    }

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
