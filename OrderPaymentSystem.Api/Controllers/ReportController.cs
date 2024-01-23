using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderPaymentSystem.DAL;
using OrderPaymentSystem.Domain.Dto.Report;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Api.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        /// <summary>
        /// Получение отчётов по ID
        /// </summary>
        /// <param name="id"></param>
        /// <remarks>
        /// Request for getting reports
        /// 
        ///     GET
        ///     {
        ///         "id": 1
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Если отчёт был получен</response>
        /// <response code="400">Если отчёт не был получен</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<ReportDto>>> GetReport(long id)
        {
            var response = await _reportService.GetReportByIdAsync(id);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        /// <summary>
        /// Получение отчётов пользователя по ID
        /// </summary>
        /// <param name="employeeId"></param>
        /// <remarks>
        /// Request for getting employee reports
        /// 
        ///     GET
        ///     {
        ///         "employeeid": 1
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Если отчёт был получен</response>
        /// <response code="400">Если отчёт не был получен</response>
        [HttpGet("reports/{employeeId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<ReportDto>>> GetEmployeeReports(long employeeId)
        {
            var response = await _reportService.GetReportsAsync(employeeId);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        /// <summary>
        /// Удаление отчёта
        /// </summary>
        /// <param name="id"></param>
        /// <remarks>
        /// Request for delete report
        /// 
        ///     DELETE
        ///     {
        ///         "id": 1
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Если отчёт удалился</response>
        /// <response code="400">Если отчёт не был удалён</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<ReportDto>>> Delete(long id)
        {
            var response = await _reportService.DeleteReportAsync(id);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        /// <summary>
        /// Создание отчёта
        /// </summary>
        /// <param name="dto"></param>
        /// <remarks>
        /// Request for create report
        /// 
        ///     POST
        ///     {
        ///         "name": "Test#1",
        ///         "totalrevenues": 10,
        ///         "numbersoforder": 10,
        ///         "employeeid": 1
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Если отчёт создался</response>
        /// <response code="400">Если отчёт не был создан</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<ReportDto>>> Create([FromBody] CreateReportDto dto)
        {
            var response = await _reportService.CreateReportAsync(dto);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        /// <summary>
        /// Обновление отчёта
        /// </summary>
        /// <param name="dto"></param>
        /// <remarks>
        /// Request for update report
        /// 
        ///     PUT
        ///     {
        ///         "id"": 1
        ///         "name": "Test#1"
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Если отчёт обновился</response>
        /// <response code="400">Если отчёт не был обновлён</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<ReportDto>>> Update([FromBody] UpdateReportDto dto)
        {
            var response = await _reportService.UpdateReportAsync(dto);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
