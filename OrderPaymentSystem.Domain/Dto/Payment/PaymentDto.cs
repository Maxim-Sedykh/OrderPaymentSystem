using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Domain.Dto.Payment
{
    public record PaymentDto(
            long Id,
            long BasketId,
            decimal AmountOfPayment,
            PaymentMethod PaymentMethod,
            DateTime CreatedAt
        );
}
