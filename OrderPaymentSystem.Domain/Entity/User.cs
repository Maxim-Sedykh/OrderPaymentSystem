using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Domain.Entity
{
    public class User : IEntityId<long>, IAuditable
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Patronymic { get; set; }

        public int Login { get; set; }

        public string Password { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public DateTime CreatedAt { get; set; }

        public long CreatedBy { get; set; }

        public DateTime UpdatedAt { get; set; }

        public long UpdatedBy { get; set; }

        public Employee Employee { get; set; }

        public Customer Customer { get; set; }
    }
}
