using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using OrderPaymentSystem.Application.DTOs;
using OrderPaymentSystem.Application.DTOs.OrderItem;
using OrderPaymentSystem.Application.Interfaces.Services;

namespace OrderPaymentSystem.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/orders")]
public class OrderItemsController : ControllerBase
{
    private readonly IOrderItemService _orderItemService;

    public OrderItemsController(IOrderItemService orderItemService)
    {
        _orderItemService = orderItemService;
    }

    [HttpPost("{orderId}/items")]
    public async Task<ActionResult<OrderItemDto>> Create(long orderId, CreateOrderItemDto dto, CancellationToken cancellationToken)
    {
        var response = await _orderItemService.CreateAsync(orderId, dto, cancellationToken);
        if (response.IsSuccess)
        {
            return CreatedAtAction(nameof(GetByOrderId), response.Data.OrderId, response.Data);
        }
        return BadRequest(response.Error);
    }

    [HttpPatch("items/{id}")]
    public async Task<ActionResult<OrderItemDto>> UpdateQuantity(long id, UpdateQuantityDto dto, CancellationToken cancellationToken)
    {
        var response = await _orderItemService.UpdateQuantityAsync(id, dto, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
    }

    [HttpDelete("items/{id}")]
    public async Task<ActionResult> DeleteById(long id, CancellationToken cancellationToken)
    {
        var response = await _orderItemService.DeleteByIdAsync(id, cancellationToken);
        if (response.IsSuccess)
        {
            return NoContent();
        }
        return BadRequest(response.Error);
    }

    [HttpGet("{orderId}/items")]
    public async Task<ActionResult<IEnumerable<OrderItemDto>>> GetByOrderId(long orderId, CancellationToken cancellationToken = default)
    {
        var response = await _orderItemService.GetByOrderIdAsync(orderId, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
    }
}
