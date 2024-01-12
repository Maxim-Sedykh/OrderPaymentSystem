using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Domain.Interfaces.Validations
{
    public interface IReportValidator : IBaseValidator<Report>
    {
        /// <summary>
        /// Проверяется наличие отчёта, если отчёт с переданным названием есть в БД, то сознать точно такой же нельзя
        /// Проверяется сотрудник, если с employeeId сотрудник не найден, то такого сотрудника нет 
        /// </summary>
        /// <param name="report"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        BaseResult CreateValidator(Report report, Employee employee);
    }
}
