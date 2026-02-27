using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderPaymentSystem.Api.Controllers.Abstract;
using OrderPaymentSystem.Application.DTOs;
using OrderPaymentSystem.Application.DTOs.Basket;
using OrderPaymentSystem.Application.Interfaces.Services;

namespace OrderPaymentSystem.Api.Controllers;

/// <summary>
/// Контроллер для работы с элементами корзины
/// </summary>
[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/basket")]
public class BasketItemsController : PrincipalAccessController
{
    private readonly IBasketItemService _basketItemService;

    /// <summary>
    /// Конструктор контроллера.
    /// </summary>
    /// <param name="basketItemService">Сервис для работы с элементами корзины.</param>
    public BasketItemsController(IBasketItemService basketItemService)
    {
        _basketItemService = basketItemService;
    }

    /// <summary>
    /// Создать элемент корзины
    /// </summary>
    /// <param name="dto">Модель данных для создания элемента</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>DTO созданного элемента</returns>
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

    /// <summary>
    /// Обновить количество товара в элементе корзины
    /// </summary>
    /// <param name="basketItemId">Id элемента корзины</param>
    /// <param name="dto">Модель данных для обновления количества товара</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Обновлённая модель данных элемента</returns>
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

    /// <summary>
    /// Удалить элемент корзины по его Id
    /// </summary>
    /// <param name="basketItemId">Id элемента корзины</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
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

    /// <summary>
    /// Получить всю корзину пользователя
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция элементов корзины текущего пользователя</returns>
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
