using OrderPaymentSystem.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Domain.Dto.Report
{
    public record CreateReportDto(string Name, decimal TotalRevenues, int NumbersOfOrder, long EmployeeId);
}
