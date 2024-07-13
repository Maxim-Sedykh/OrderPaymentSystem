using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using OrderPaymentSystem.Application.Validations.FluentValidations.Product;
using OrderPaymentSystem.Domain.Constants;
using OrderPaymentSystem.Domain.Dto.Product;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Api.Controllers
{
    /// <summary>
    /// Контроллер для работы с товарами
    /// </summary>
    //[Authorize(Roles = "Admin")]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly UpdateProductValidator _updateProductValidator;
        private readonly CreateProductValidator _createProductValidator;

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
        public async Task<ActionResult<BaseResult<ProductDto>>> GetProduct(int id)
        {
            var response = await _productService.GetProductByIdAsync(id);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        /// <summary>
        /// Получение всех товаров
        /// </summary>
        /// <remarks>
        /// Request for getting all products
        /// </remarks>
        /// <response code="200">Если товары были получены</response>
        /// <response code="400">Если товары не были получены</response>
        [HttpGet(RouteConstants.GetProducts)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<ProductDto>>> GetAllProducts()
        {
            var response = await _productService.GetProductsAsync();
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        /// <summary>
        /// Удаление товара
        /// </summary>
        /// <param name="id"></param>
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
        [HttpDelete(RouteConstants.DeleteProductById)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<ProductDto>>> DeleteProduct(int id)
        {
            var response = await _productService.DeleteProductAsync(id);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        /// <summary>
        /// Создание товара
        /// </summary>
        /// <param name="dto"></param>
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
        [HttpPost(RouteConstants.CreateProduct)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<ProductDto>>> CreateProduct([FromBody] CreateProductDto dto)
        {
            var validationResult = await _createProductValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var response = await _productService.CreateProductAsync(dto);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        /// <summary>
        /// Обновление товара
        /// </summary>
        /// <param name="dto"></param>
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
        [HttpPut(RouteConstants.UpdateProduct)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<ProductDto>>> UpdateProduct([FromBody] UpdateProductDto dto)
        {
            var validationResult = await _updateProductValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var response = await _productService.UpdateProductAsync(dto);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
