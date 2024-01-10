using OrderPaymentSystem.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Domain.Entity
{
    public class Customer : IEntityId<long>, IAuditable
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        public User User { get; set; }

        public List<Order> Orders { get; set; }

        public long OrdersMade { get; set; }

        public DateTime CreatedAt { get; set; }

        public long CreatedBy { get; set; }

        public DateTime UpdatedAt { get; set; }

        public long UpdatedBy { get; set; }
    }
}
