using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Domain.Dto.Report
{
    public record ReportDto(long Id, string Name, decimal TotalRevenues, int NumberOfOrders, string CreatedAt)
    {
    }
}
