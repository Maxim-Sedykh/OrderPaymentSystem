using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Domain.Entity
{
    public class Payment : IEntityId<long>, IAuditable
    {
        public long Id { get; set; }

        public long BasketId { get; set; }

        public Basket Basket { get; set; }

        public decimal CostOfOrders { get; set; }

        public decimal AmountOfPayment { get; set; }

        public decimal CashChange { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public DateTime CreatedAt { get; set; }

        public long CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public long? UpdatedBy { get; set; }
    }
}
