using OrderPaymentSystem.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Domain.Dto.Order
{
    public record OrderDto(
            long Id,
            long UserId,
            long BasketId,
            long ProductId,
            int ProductCount,
            string CreatedAt
        );
}
