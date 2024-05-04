using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderPaymentSystem.Application.Validations.FluentValidations.Payment;
using OrderPaymentSystem.Domain.Dto.Order;
using OrderPaymentSystem.Domain.Dto.Payment;
using OrderPaymentSystem.Domain.Dto.Product;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Api.Controllers
{
    /// <summary>
    /// Контроллер для работы с платежами
    /// </summary>
    //[Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly UpdatePaymentValidation _updatePaymentValidator;
        private readonly CreatePaymentValidator _createPaymentValidator;

        public PaymentController(IPaymentService paymentService, UpdatePaymentValidation updatePaymentValidator,
            CreatePaymentValidator createPaymentValidator)
        {
            _paymentService = paymentService;
            _updatePaymentValidator = updatePaymentValidator;
            _createPaymentValidator = createPaymentValidator;
        }

        /// <summary>
        /// Получение платежа по ID
        /// </summary>
        /// <param name="id"></param>
        /// <remarks>
        /// Request for getting payment
        /// 
        ///     GET
        ///     {
        ///         "id": 1
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Если платеж был получен</response>
        /// <response code="400">Если платеж не был получен</response>
        [HttpGet("payment/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<PaymentDto>>> GetPayment(long id)
        {
            var response = await _paymentService.GetPaymentByIdAsync(id);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        /// <summary>
        /// Получение всех платежей пользователя
        /// </summary>
        /// <param name="userId"></param>
        ///<remarks>
        /// Request for getting user payments
        /// 
        ///     DELETE
        ///     {
        ///         "userid": 1
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Если платежи пользователя были получены</response>
        /// <response code="400">Если платежи пользователя не были получены</response>
        [HttpGet("get-user-payments/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CollectionResult<PaymentDto>>> GetUserPayments(long userId)
        {
            var response = await _paymentService.GetUserPaymentsAsync(userId);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        /// <summary>
        /// Удаление платежа по идентификатору
        /// </summary>
        /// <param name="id"></param>
        /// <remarks>
        /// Request for delete payment
        /// 
        ///     DELETE
        ///     {
        ///         "id": 1
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Если платёж удалился</response>
        /// <response code="400">Если платёж не был удалён</response>
        [HttpDelete("delete-payment/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<PaymentDto>>> DeletePayment(long id)
        {
            var response = await _paymentService.DeletePaymentAsync(id);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        /// <summary>
        /// Создание платежа
        /// </summary>
        /// <param name="dto"></param>
        /// <remarks>
        /// Request for create payment
        ///     
        ///     POST
        ///     {
        ///         "basketId": 0,
        ///         "amountOfPayment": 0,
        ///         "paymentMethod": 0,
        ///         "street": "string",
        ///         "city": "string",
        ///         "country": "string",
        ///         "zipcode": "string"
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Если платеж создался</response>
        /// <response code="400">Если платеж не был создан</response>
        [HttpPost("create-payment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<PaymentDto>>> CreatePayment([FromBody] CreatePaymentDto dto)
        {
            var validationResult = await _createPaymentValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var response = await _paymentService.CreatePaymentAsync(dto);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        /// <summary>
        /// Обновление платежа
        /// </summary>
        /// <param name="dto"></param>
        /// <remarks>
        /// Request for update payment
        /// 
        ///     PUT
        ///     {
        ///         "id": 1
        ///         "amountofpayment": 4000,
        ///         "paymentmethod": 2
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Если платёж обновился</response>
        /// <response code="400">Если платёж не был обновлён</response>
        [HttpPut("update-payment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<ProductDto>>> UpdatePayment([FromBody] UpdatePaymentDto dto)
        {
            var validationResult = await _updatePaymentValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var response = await _paymentService.UpdatePaymentAsync(dto);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        /// <summary>
        /// Получение заказов определённого платежа по идентификатору
        /// </summary>
        /// <param name="id"></param>
        /// <remarks>
        /// Request for getting payment orders
        /// 
        ///     PUT
        ///     {
        ///         "id": 1
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Если заказы платежа были получены</response>
        /// <response code="400">Если заказы платежа не были получены</response>
        [HttpGet("get-payment-orders/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CollectionResult<OrderDto>>> GetPaymentOrders(long id)
        {
            var response = await _paymentService.GetPaymentOrdersAsync(id);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
