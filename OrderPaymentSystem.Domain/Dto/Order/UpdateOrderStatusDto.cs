using OrderPaymentSystem.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderPaymentSystem.Domain.Dto.Order
{
    public record UpdateOrderStatusDto
    {
        public OrderStatus NewStatus { get; set; }
    }
}
