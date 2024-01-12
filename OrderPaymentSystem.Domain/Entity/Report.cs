using OrderPaymentSystem.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Domain.Entity
{
    public class Report: IEntityId<long>, IAuditable
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public decimal TotalRevenues { get; set; }

        public int NumberOfOrders { get; set; }

        public DateTime CreatedAt { get; set; }

        public long CreatedBy { get; set; }

        public DateTime UpdatedAt { get; set; }

        public long UpdatedBy { get; set; }

        public long EmployeeId { get; set; }

        public Employee Employee { get; set; }
    }
}
