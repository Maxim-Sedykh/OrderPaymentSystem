using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderPaymentSystem.Api.Controllers.Abstract;
using OrderPaymentSystem.Application.DTOs.Order;
using OrderPaymentSystem.Application.Interfaces.Services;

namespace OrderPaymentSystem.Api.Controllers;

/// <summary>
/// Контроллер для работы с заказами
/// </summary>
[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/orders")]
public class OrdersController : PrincipalAccessController
{
    private readonly IOrderService _orderService;

    /// <summary>
    /// Конструктор контроллера
    /// </summary>
    /// <param name="orderService"></param>
    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Получить заказ по Id
    /// </summary>
    /// <param name="id">Id заказа</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns><see cref="OrderDto"/></returns>
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

    /// <summary>
    /// Создать заказ
    /// </summary>
    /// <param name="dto">Модель создания заказа</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Id созданного заказа</returns>
    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create(CreateOrderDto dto, CancellationToken cancellationToken)
    {
        var response = await _orderService.CreateAsync(AuthorizedUserId, dto, cancellationToken);
        if (response.IsSuccess)
        {
            return CreatedAtAction(nameof(GetById), new { id = response.Data }, response.Data);
        }
        return BadRequest(response.Error);
    }

    /// <summary>
    /// Обновить статус заказа
    /// </summary>
    /// <param name="id">Id заказа</param>
    /// <param name="dto">Модель обновления статуса заказа</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    [HttpPatch("{id}/status")]
    public async Task<ActionResult> UpdateStatus(long id, UpdateOrderStatusDto dto, CancellationToken cancellationToken)
    {
        var response = await _orderService.UpdateStatusAsync(id, dto, cancellationToken);
        if (response.IsSuccess)
        {
            return NoContent();
        }
        return BadRequest(response.Error);
    }

    /// <summary>
    /// Получить все заказы текущего пользователя
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Заказы пользователя в виде <see cref="OrderDto"/></returns>
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

    /// <summary>
    /// Завершить обработку заказа, связать платёж и заказ
    /// </summary>
    /// <param name="id">Id заказа</param>
    /// <param name="paymentId">Id платежа</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    [HttpPost("{id}/payments/{paymentId}/complete")]
    public async Task<ActionResult> CompleteProcessing(long id, long paymentId, CancellationToken cancellationToken = default)
    {
        var response = await _orderService.CompleteProcessingAsync(id, paymentId, cancellationToken);
        if (response.IsSuccess)
        {
            return NoContent();
        }
        return BadRequest(response.Error);
    }

    /// <summary>
    /// Массовое обновление элементов заказа
    /// </summary>
    /// <param name="orderId">Id заказа</param>
    /// <param name="dto">Модель для обновления элементов заказа</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
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

    /// <summary>
    /// Обновить статус заказа на "Доставлен"
    /// </summary>
    /// <param name="id">Id заказа</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    [HttpPost("{id}/ship")]
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
