using OrderPaymentSystem.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Domain.Entity
{
    public class Report: IEntityId<long>
    {
        public long Id { get; set; }

        public int TotalRevenues { get; set; }

        public int Profit { get; set; }

        public int NumberOfOrders { get; set; }

        public DateTime CreatedAt { get; set; }

        public long CreatedBy { get; set; }

        public DateTime UpdatedAt { get; set; }

        public long UpdatedBy { get; set; }

        public long UserId { get; set; }

        public Employee Employee { get; set; }
    }
}
