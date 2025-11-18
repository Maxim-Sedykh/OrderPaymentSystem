using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderPaymentSystem.Application.Validations.FluentValidations.Product;
using OrderPaymentSystem.Domain.Constants;
using OrderPaymentSystem.Domain.Dto.Product;
using OrderPaymentSystem.Domain.Interfaces.Services;

namespace OrderPaymentSystem.Api.Controllers;

/// <summary>
/// Контроллер для работы с товарами
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly UpdateProductValidator _updateProductValidator;
    private readonly CreateProductValidator _createProductValidator;

    /// <summary>
    /// Конструктор контроллера для работы с товарами
    /// </summary>
    /// <param name="productService"></param>
    /// <param name="updateProductValidator"></param>
    /// <param name="createProductValidator"></param>
    public ProductController(IProductService productService, UpdateProductValidator updateProductValidator,
        CreateProductValidator createProductValidator)
    {
        _productService = productService;
        _updateProductValidator = updateProductValidator;
        _createProductValidator = createProductValidator;
    }

    /// <summary>
    /// Получение товара по ID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    /// <remarks>
    /// Request for getting products
    /// 
    ///     GET
    ///     {
    ///         "id": 1
    ///     }
    ///     
    /// </remarks>
    /// <response code="200">Если товар был получен</response>
    /// <response code="400">Если товар не был получен</response>
    [HttpGet(RouteConstants.GetProductById)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> GetProduct(int id, CancellationToken cancellationToken)
    {
        var response = await _productService.GetProductByIdAsync(id, cancellationToken);
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
    /// <remarks>
    /// Request for getting all products
    /// </remarks>
    /// <response code="200">Если товары были получены</response>
    /// <response code="400">Если товары не были получены</response>
    [HttpGet(RouteConstants.GetProducts)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> GetAllProducts(CancellationToken cancellationToken)
    {
        var response = await _productService.GetProductsAsync(cancellationToken);
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
    /// Request for delete product
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
    [HttpDelete(RouteConstants.DeleteProductById)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> DeleteProduct(int id, CancellationToken cancellationToken)
    {
        var response = await _productService.DeleteProductAsync(id, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
    }

    /// <summary>
    /// Создание товара
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    /// <remarks>
    /// Request for create product
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
    [HttpPost(RouteConstants.CreateProduct)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _createProductValidator.ValidateAsync(dto, cancellationToken);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var response = await _productService.CreateProductAsync(dto, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
    }

    /// <summary>
    /// Обновление товара
    /// </summary>
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
    [HttpPut(RouteConstants.UpdateProduct)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> UpdateProduct(UpdateProductDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _updateProductValidator.ValidateAsync(dto, cancellationToken);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var response = await _productService.UpdateProductAsync(dto, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
    }
}
