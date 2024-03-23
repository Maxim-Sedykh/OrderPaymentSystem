using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Domain.Dto.Basket
{
    public record BasketDto(long Id, long UserId, string CreatedAt, decimal CostOfAllOrders);
}
