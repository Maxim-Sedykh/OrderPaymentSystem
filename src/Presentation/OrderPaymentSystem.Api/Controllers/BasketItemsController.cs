using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderPaymentSystem.Api.Controllers.Abstract;
using OrderPaymentSystem.Application.DTOs;
using OrderPaymentSystem.Application.DTOs.Basket;
using OrderPaymentSystem.Application.Interfaces.Services;

namespace OrderPaymentSystem.Api.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/basket")]
public class BasketItemsController : PrincipalAccessController
{
    private readonly IBasketItemService _basketItemService;

    public BasketItemsController(IBasketItemService basketItemService)
    {
        _basketItemService = basketItemService;
    }

    [HttpPost]
    public async Task<ActionResult<BasketItemDto>> Create(CreateBasketItemDto dto, CancellationToken cancellationToken)
    {
        var response = await _basketItemService.CreateAsync(AuthorizedUserId, dto, cancellationToken);
        if (response.IsSuccess)
        {
            return CreatedAtAction(nameof(GetByUserId), response.Data);
        }
        return BadRequest(response.Error);
    }

    [HttpPatch("{basketItemId}")]
    public async Task<ActionResult<BasketItemDto>> UpdateQuantity(long basketItemId, UpdateQuantityDto dto, CancellationToken cancellationToken)
    {
        var response = await _basketItemService.UpdateQuantityAsync(basketItemId, dto, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
    }

    [HttpDelete("{basketItemId}")]
    public async Task<ActionResult> DeleteById(long basketItemId, CancellationToken cancellationToken)
    {
        var response = await _basketItemService.DeleteByIdAsync(basketItemId, cancellationToken);
        if (response.IsSuccess)
        {
            return NoContent();
        }
        return BadRequest(response.Error);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BasketItemDto>>> GetByUserId(CancellationToken cancellationToken = default)
    {
        var response = await _basketItemService.GetByUserIdAsync(AuthorizedUserId, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return NotFound(response.Error);
    }
}
