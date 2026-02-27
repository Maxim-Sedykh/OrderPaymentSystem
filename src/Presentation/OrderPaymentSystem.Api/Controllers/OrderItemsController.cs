using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderPaymentSystem.Application.DTOs;
using OrderPaymentSystem.Application.DTOs.OrderItem;
using OrderPaymentSystem.Application.Interfaces.Services;

namespace OrderPaymentSystem.Api.Controllers;

/// <summary>
/// Контроллер для работы с элементами заказа
/// </summary>
[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/orders")]
public class OrderItemsController : ControllerBase
{
    private readonly IOrderItemService _orderItemService;

    /// <summary>
    /// Конструктор контроллера.
    /// </summary>
    /// <param name="orderItemService">Сервис для работы с элементами заказа.</param>
    public OrderItemsController(IOrderItemService orderItemService)
    {
        _orderItemService = orderItemService;
    }

    /// <summary>
    /// Создать элемент заказа
    /// </summary>
    /// <param name="orderId">Id заказа</param>
    /// <param name="dto">Модель данных для создания элемента</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>DTO созданного элемента</returns>
    [HttpPost("{orderId}/items")]
    public async Task<ActionResult<OrderItemDto>> Create(long orderId, CreateOrderItemDto dto, CancellationToken cancellationToken)
    {
        var response = await _orderItemService.CreateAsync(orderId, dto, cancellationToken);
        if (response.IsSuccess)
        {
            return CreatedAtAction(nameof(GetByOrderId), new { orderId = response.Data!.OrderId }, response.Data);
        }
        return BadRequest(response.Error);
    }

    /// <summary>
    /// Обновить количество товара в элементе заказа
    /// </summary>
    /// <param name="id">Id элемента заказа</param>
    /// <param name="dto">Модель данных для обновления количества товара</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Обновлённая модель данных элемента</returns>
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

    /// <summary>
    /// Удалить элемент заказа по его Id
    /// </summary>
    /// <param name="id">Id элемента заказа</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
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

    /// <summary>
    /// Получить элементы по Id заказа
    /// </summary>
    /// <param name="orderId">Id заказа</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция элементов заказа</returns>
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
