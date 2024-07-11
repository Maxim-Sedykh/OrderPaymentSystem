using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Domain.Dto.Order
{
    public record UpdateOrderDto(
            long Id,
            int ProductId,
            int ProductCount
        );
}
