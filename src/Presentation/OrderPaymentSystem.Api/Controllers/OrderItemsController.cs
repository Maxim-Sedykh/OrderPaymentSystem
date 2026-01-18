using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using OrderPaymentSystem.Application.DTOs;
using OrderPaymentSystem.Application.DTOs.Basket;
using OrderPaymentSystem.Application.DTOs.OrderItem;
using OrderPaymentSystem.Application.Interfaces.Services;

namespace OrderPaymentSystem.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class OrderItemsController : ControllerBase
{
    private readonly IOrderItemService _orderItemService;

    public OrderItemsController(IOrderItemService orderItemService)
    {
        _orderItemService = orderItemService;
    }

    [HttpPost]
    public async Task<ActionResult<OrderItemDto>> Create(CreateOrderItemDto dto, CancellationToken cancellationToken)
    {
        var response = await _orderItemService.CreateAsync(dto, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
    }

    [HttpPatch("/{orderItemId}")]
    public async Task<ActionResult<BasketItemDto>> UpdateQuantity(long orderItemId, UpdateQuantityDto dto, CancellationToken cancellationToken)
    {
        var response = await _orderItemService.UpdateQuantityAsync(orderItemId, dto, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
    }

    [HttpDelete("/{basketItemId}")]
    public async Task<ActionResult> DeleteById(long orderItemId, CancellationToken cancellationToken)
    {
        var response = await _orderItemService.DeleteByIdAsync(orderItemId, cancellationToken);
        if (response.IsSuccess)
        {
            return NoContent();
        }
        return BadRequest(response.Error);
    }

    [HttpGet("/{orderId}")]
    public async Task<ActionResult<BasketItemDto[]>> GetByOrderId(long orderId, CancellationToken cancellationToken = default)
    {
        var response = await _orderItemService.GetByOrderIdAsync(orderId, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
    }
}
