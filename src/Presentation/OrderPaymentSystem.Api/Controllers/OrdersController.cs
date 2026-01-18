using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using OrderPaymentSystem.Api.Controllers.Abstract;
using OrderPaymentSystem.Application.DTOs.Basket;
using OrderPaymentSystem.Application.DTOs.Order;
using OrderPaymentSystem.Application.Interfaces.Services;

namespace OrderPaymentSystem.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class OrdersController : PrincipalAccessController
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create(CreateOrderDto dto, CancellationToken cancellationToken)
    {
        var response = await _orderService.CreateAsync(AuthorizedUserId, dto, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
    }

    [HttpGet("/{orderId}")]
    public async Task<ActionResult<OrderDto>> GetById(long orderId, CancellationToken cancellationToken)
    {
        var response = await _orderService.GetByIdAsync(orderId, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
    }

    [HttpPatch("/{orderId}/status")]
    public async Task<ActionResult> UpdateStatus(long orderId, UpdateOrderStatusDto dto, CancellationToken cancellationToken)
    {
        var response = await _orderService.UpdateStatusAsync(orderId, dto, cancellationToken);
        if (response.IsSuccess)
        {
            return NoContent();
        }
        return BadRequest(response.Error);
    }

    [HttpGet]
    public async Task<ActionResult<OrderDto[]>> GetByUserId(CancellationToken cancellationToken = default)
    {
        var response = await _orderService.GetByUserIdAsync(AuthorizedUserId, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
    }

    [HttpGet("/{orderId}/{paymentId}")]
    public async Task<ActionResult<BasketItemDto[]>> CompleteProcessing(long orderId, long paymentId, CancellationToken cancellationToken = default)
    {
        var response = await _orderService.CompleteProcessingAsync(orderId, paymentId, cancellationToken);
        if (response.IsSuccess)
        {
            return NoContent();
        }
        return BadRequest(response.Error);
    }

    [HttpPatch("/{orderId}")]
    public async Task<ActionResult<BasketItemDto[]>> UpdateBulkOrderItems(long orderId, UpdateBulkOrderItemsDto dto, CancellationToken cancellationToken = default)
    {
        var response = await _orderService.UpdateBulkOrderItemsAsync(orderId, dto, cancellationToken);
        if (response.IsSuccess)
        {
            return NoContent();
        }
        return BadRequest(response.Error);
    }

    [HttpPost("/{orderId}")]
    public async Task<ActionResult<BasketItemDto[]>> ShipOrder(long orderId, CancellationToken cancellationToken = default)
    {
        var response = await _orderService.ShipOrderAsync(orderId, cancellationToken);
        if (response.IsSuccess)
        {
            return NoContent();
        }
        return BadRequest(response.Error);
    }
}
