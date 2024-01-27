using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderPaymentSystem.DAL;
using OrderPaymentSystem.Domain.Dto.Product;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Api.Controllers
{
    //[Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
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
        [HttpGet("{id}")]
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
        [HttpGet("products")]
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
        [HttpDelete("{id}")]
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
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<ProductDto>>> CreateProduct([FromBody] CreateProductDto dto)
        {
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
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<ProductDto>>> UpdateProduct([FromBody] UpdateProductDto dto)
        {
            var response = await _productService.UpdateProductAsync(dto);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
