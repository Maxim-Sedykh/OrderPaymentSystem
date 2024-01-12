using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces.Validations;
using OrderPaymentSystem.Domain.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Application.Validations
{
    public class ReportValidator : IReportValidator
    {
        public BaseResult CreateValidator(Report report, Employee employee)
        {
            if (report != null)
            {
                return new BaseResult()
                {
                    ErrorMessage = ErrorMessage.ReportAlreadyExist,
                    ErrorCode = (int)ErrorCodes.ReportAlreadyExist
                };
            }

            if (employee ==  null)
            {
                return new BaseResult()
                {
                    ErrorMessage = ErrorMessage.EmployeeNotFound,
                    ErrorCode = (int)ErrorCodes.EmployeeNotFound
                };
            }
            return new BaseResult();
        }

        public BaseResult ValidateOnNull(Report model)
        {
            if (model == null)
            {
                return new BaseResult()
                {
                    ErrorMessage = ErrorMessage.ReportNotFound,
                    ErrorCode = (int)ErrorCodes.ReportNotFound
                };
            }
            return new BaseResult();
        }
    }
}
