using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Domain.Enum
{
    public enum ErrorCodes
    {
        ReportsNotFound = 0,
        ReportNotFound = 1,
        ReportAlreadyExist = 2,

        EmployeeNotFound = 10,

        InternalServerError = 500
    }
}
