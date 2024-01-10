using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Domain.Entity
{
    public class Order : IEntityId<long>, IAuditable
    {
        public long Id { get; set; }

        public long CustomerId { get; set; }

        public Customer Customer { get; set; }

        public long EmployeeId { get; set; }

        public Employee Employee { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; }

        public Payment Payment { get; set; }

        public decimal OrderPrice { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public DateTime CreatedAt { get; set; }

        public long CreatedBy { get; set; }

        public DateTime UpdatedAt { get; set; }

        public long UpdatedBy { get; set; }
    }
}
