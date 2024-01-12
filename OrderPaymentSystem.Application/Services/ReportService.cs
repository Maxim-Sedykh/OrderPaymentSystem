using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.Domain.Dto.Report;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Interfaces.Validations;
using OrderPaymentSystem.Domain.Result;
using Serilog;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace OrderPaymentSystem.Application.Services
{
    public class ReportService: IReportService
    {
        private readonly IBaseRepository<Report> _reportRepository;
        private readonly IBaseRepository<Employee> _employeeRepository;
        private readonly IBaseRepository<Order> _orderRepository;
        private readonly IMapper _mapper;
        private readonly IReportValidator _reportValidator;
        private readonly ILogger _logger;

        public ReportService(IBaseRepository<Report> reportRepository, ILogger logger, IBaseRepository<Employee> employeeRepository,
            IReportValidator reportValidator, IBaseRepository<Order> orderRepository, IMapper mapper)
        {
            _reportRepository = reportRepository;
            _logger = logger;
            _employeeRepository = employeeRepository;
            _reportValidator = reportValidator;
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task<BaseResult<ReportDto>> CreateReportAsync(CreateReportDto dto)
        {
            try
            {
                var employee = await _employeeRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.EmployeeId);
                var report = await _reportRepository.GetAll().FirstOrDefaultAsync(x => x.Name == dto.Name);

                var result = _reportValidator.CreateValidator(report, employee);
                if (!result.IsSuccess)
                {
                    return new BaseResult<ReportDto>()
                    {
                        ErrorMessage = result.ErrorMessage,
                        ErrorCode = result.ErrorCode
                    };
                }

                Order[] orders = await _orderRepository.GetAll()
                    .Where(x => x.EmployeeId == dto.EmployeeId)
                    .Select(x => x).ToArrayAsync();

                var totalRevenues = 0m;

                foreach (var order in orders)
                {
                    totalRevenues = +order.OrderPrice;
                }

                report = new Report()
                {
                    Name = dto.Name,
                    TotalRevenues = totalRevenues,
                    NumberOfOrders = orders.Length,
                    EmployeeId = employee.Id
                };

                await _reportRepository.CreateAsync(report);
                return new BaseResult<ReportDto>()
                {
                    Data = _mapper.Map<ReportDto>(report),
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return new BaseResult<ReportDto>()
                {
                    ErrorMessage = ErrorMessage.InternalServerError,
                    ErrorCode = (int)ErrorCodes.InternalServerError
                };
            }
        }

        /// <inheritdoc/>
        public async Task<BaseResult<ReportDto>> DeleteReportAsync(long id)
        {
            try
            {
                var report = await _reportRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
                var result = _reportValidator.ValidateOnNull(report);

                if (!result.IsSuccess)
                {
                    return new BaseResult<ReportDto>()
                    {
                        ErrorMessage = result.ErrorMessage,
                        ErrorCode = result.ErrorCode
                    };
                }
                await _reportRepository.RemoveAsync(report);
                return new BaseResult<ReportDto>()
                {
                    Data = _mapper.Map<ReportDto>(report),
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return new BaseResult<ReportDto>()
                {
                    ErrorMessage = ErrorMessage.InternalServerError,
                    ErrorCode = (int)ErrorCodes.InternalServerError
                };
            }
        }

        /// <inheritdoc/>
        public async Task<BaseResult<ReportDto>> GerReportByIdAsync(long id)
        {
            ReportDto? report;
            try
            {
                report = await _reportRepository.GetAll()
                    .Select(x => new ReportDto(x.Id, x.Name, x.TotalRevenues, x.NumberOfOrders, x.CreatedAt.ToLongDateString()))
                    .FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return new BaseResult<ReportDto>()
                {
                    ErrorMessage = ErrorMessage.InternalServerError,
                    ErrorCode = (int)ErrorCodes.InternalServerError
                };
            }

            if (report == null)
            {
                _logger.Warning($"Отчёт с {id} не найден", id);
                return new BaseResult<ReportDto>()
                {
                    ErrorMessage = ErrorMessage.ReportNotFound,
                    ErrorCode = (int)ErrorCodes.ReportNotFound
                };
            }

            return new BaseResult<ReportDto>()
            {
                Data = report,
            };
        }

        /// <inheritdoc/>
        public async Task<CollectionResult<ReportDto>> GerReportsAsync(long employeeId)
        {
            ReportDto[] reports;
            try
            {
                reports = await _reportRepository.GetAll()
                    .Where(x => x.EmployeeId == employeeId)
                    .Select(x => new ReportDto(x.Id, x.Name, x.TotalRevenues, x.NumberOfOrders, x.CreatedAt.ToLongDateString()))
                    .ToArrayAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return new CollectionResult<ReportDto>()
                {
                    ErrorMessage = ErrorMessage.InternalServerError,
                    ErrorCode = (int)ErrorCodes.InternalServerError
                };
            }

            if (!reports.Any())
            {
                _logger.Warning(ErrorMessage.ReportsNotFound, reports.Length);
                return new CollectionResult<ReportDto>()
                {
                    ErrorMessage = ErrorMessage.ReportsNotFound,
                    ErrorCode = (int)ErrorCodes.ReportsNotFound
                };
            }

            return new CollectionResult<ReportDto>()
            {
                Data = reports,
                Count = reports.Length
            };
        }

        /// <inheritdoc/>
        public async Task<BaseResult<ReportDto>> UpdateReportAsync(UpdateReportDto dto)
        {
            try
            {
                var report = await _reportRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.Id);
                var result = _reportValidator.ValidateOnNull(report);

                if (!result.IsSuccess)
                {
                    return new BaseResult<ReportDto>()
                    {
                        ErrorMessage = result.ErrorMessage,
                        ErrorCode = result.ErrorCode
                    };
                }
                report.Name = dto.Name;

                await _reportRepository.UpdateAsync(report);
                return new BaseResult<ReportDto>()
                {
                    Data = _mapper.Map<ReportDto>(report),
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return new BaseResult<ReportDto>()
                {
                    ErrorMessage = ErrorMessage.InternalServerError,
                    ErrorCode = (int)ErrorCodes.InternalServerError
                };
            }
        }
    }
}
