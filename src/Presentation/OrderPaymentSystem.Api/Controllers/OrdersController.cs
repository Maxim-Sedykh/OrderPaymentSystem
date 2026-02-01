using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderPaymentSystem.Api.Controllers.Abstract;
using OrderPaymentSystem.Application.DTOs.Order;
using OrderPaymentSystem.Application.DTOs.OrderItem;
using OrderPaymentSystem.Application.Interfaces.Services;

namespace OrderPaymentSystem.Api.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class OrdersController : PrincipalAccessController
{
    private readonly IOrderService _orderService;
    private readonly IOrderItemService _orderItemService;

    public OrdersController(IOrderService orderService, IOrderItemService orderItemService)
    {
        _orderService = orderService;
        _orderItemService = orderItemService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var response = await _orderService.GetByIdAsync(id, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return NotFound(response.Error);
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create(CreateOrderDto dto, CancellationToken cancellationToken)
    {
        var response = await _orderService.CreateAsync(AuthorizedUserId, dto, cancellationToken);
        if (response.IsSuccess)
        {
            return CreatedAtAction(nameof(GetById), new { id = response.Data.Id }, response.Data);
        }
        return BadRequest(response.Error);
    }

    [HttpPatch("/{id}/status")]
    public async Task<ActionResult> UpdateStatus(long id, UpdateOrderStatusDto dto, CancellationToken cancellationToken)
    {
        var response = await _orderService.UpdateStatusAsync(id, dto, cancellationToken);
        if (response.IsSuccess)
        {
            return NoContent();
        }
        return BadRequest(response.Error);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetByUserId(CancellationToken cancellationToken = default)
    {
        var response = await _orderService.GetByUserIdAsync(AuthorizedUserId, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
    }

    [HttpPost("{id}/process/{paymentId}")]
    public async Task<ActionResult> CompleteProcessing(long id, long paymentId, CancellationToken cancellationToken = default)
    {
        var response = await _orderService.CompleteProcessingAsync(id, paymentId, cancellationToken);
        if (response.IsSuccess)
        {
            return NoContent();
        }
        return BadRequest(response.Error);
    }

    [HttpPatch("{orderId}/items")]
    public async Task<ActionResult> UpdateBulkOrderItems(long orderId, UpdateBulkOrderItemsDto dto, CancellationToken cancellationToken = default)
    {
        var response = await _orderService.UpdateBulkOrderItemsAsync(orderId, dto, cancellationToken);
        if (response.IsSuccess)
        {
            return NoContent();
        }
        return BadRequest(response.Error);
    }

    [HttpPost("{id}/shipment")]
    public async Task<ActionResult> ShipOrder(long id, CancellationToken cancellationToken = default)
    {
        var response = await _orderService.ShipOrderAsync(id, cancellationToken);
        if (response.IsSuccess)
        {
            return NoContent();
        }
        return BadRequest(response.Error);
    }
}
