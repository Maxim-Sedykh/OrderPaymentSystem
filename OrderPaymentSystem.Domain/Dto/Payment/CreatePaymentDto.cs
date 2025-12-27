using OrderPaymentSystem.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderPaymentSystem.Domain.Dto.Payment
{
    public class CreatePaymentDto
    {
        public long OrderId { get; set; }
        public decimal AmountToPay { get; set; }
        public decimal AmountPayed { get; set; }
        public decimal CashChange { get; set; }
        public PaymentMethod Method { get; set; }
    }
}
