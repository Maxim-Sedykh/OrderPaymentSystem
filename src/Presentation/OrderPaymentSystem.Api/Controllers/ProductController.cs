using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderPaymentSystem.Application.DTOs.Product;
using OrderPaymentSystem.Application.Interfaces.Services;

namespace OrderPaymentSystem.Api.Controllers;

/// <summary>
/// Контроллер для работы с товарами
/// </summary>
[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/products")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    /// <summary>
    /// Конструктор контроллера для работы с товарами
    /// </summary>
    /// <param name="productService"></param>
    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Получение товара по ID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    /// <remarks>
    /// 
    ///     GET
    ///     {
    ///         "id": 1
    ///     }
    ///     
    /// </remarks>
    /// <response code="200">Если товар был получен</response>
    /// <response code="400">Если товар не был получен</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var response = await _productService.GetByIdAsync(id, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
    }

    /// <summary>
    /// Получение всех товаров
    /// </summary>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    /// <response code="200">Если товары были получены</response>
    /// <response code="400">Если товары не были получены</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll(CancellationToken cancellationToken)
    {
        var response = await _productService.GetAllAsync(cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
    }

    /// <summary>
    /// Удаление товара
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    /// <remarks>
    /// 
    ///     DELETE
    ///     {
    ///         "id": 1
    ///     }
    ///     
    /// </remarks>
    /// <response code="200">Если товар удалился</response>
    /// <response code="400">Если товар не был удалён</response>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeleteProduct(int id, CancellationToken cancellationToken)
    {
        var response = await _productService.DeleteByIdAsync(id, cancellationToken);
        if (response.IsSuccess)
        {
            return NoContent();
        }
        return BadRequest(response.Error);
    }

    /// <summary>
    /// Создание товара
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    /// <remarks>
    /// 
    ///     POST
    ///     {
    ///         "productname": "Гвозди",
    ///         "description": "Хорошие гвозди большого размера",
    ///         "cost": 300
    ///     }
    ///     
    /// </remarks>
    /// <response code="200">Если товар создался</response>
    /// <response code="400">Если товар не был создан</response>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Create(CreateProductDto dto, CancellationToken cancellationToken)
    {
        var response = await _productService.CreateAsync(dto, cancellationToken);
        if (response.IsSuccess)
        {
            return Created();
        }
        return BadRequest(response.Error);
    }

    /// <summary>
    /// Обновление товара
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    /// <remarks>
    /// Request for update product
    /// 
    ///     PUT
    ///     {
    ///         "id": 1
    ///         "productname": "Шурупы",
    ///         "description": "Качественные шурупы из Китая большого размера",
    ///         "cost": 400
    ///     }
    ///     
    /// </remarks>
    /// <response code="200">Если товар обновился</response>
    /// <response code="400">Если товар не был обновлён</response>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> UpdateProduct(int id, UpdateProductDto dto, CancellationToken cancellationToken)
    {
        var response = await _productService.UpdateAsync(id, dto, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }

        return BadRequest(response.Error);
    }
}
