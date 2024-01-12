using OrderPaymentSystem.Domain.Dto.Report;
using OrderPaymentSystem.Domain.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Domain.Interfaces.Services
{
    /// <summary>
    /// Сервис отвечающий за работу с доменной части отчёта (Report)
    /// </summary>
    public interface IReportService
    {
        /// <summary>
        /// Получение всех отчётов пользователя
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<CollectionResult<ReportDto>> GerReportsAsync(long id);

        /// <summary>
        /// Получение отчёта по идентификатору
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<BaseResult<ReportDto>> GerReportByIdAsync(long id);

        /// <summary>
        /// Создание отчёта с базовыми параметрами
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<BaseResult<ReportDto>> CreateReportAsync(CreateReportDto dto);

        /// <summary>
        /// Удаление отчёта по идентификатору
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<BaseResult<ReportDto>> DeleteReportAsync(long id);

        /// <summary>
        /// Обновление отчёта
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<BaseResult<ReportDto>> UpdateReportAsync(UpdateReportDto dto);
    }
}
