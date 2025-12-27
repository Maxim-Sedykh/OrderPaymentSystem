using System;
using System.Collections.Generic;
using System.Text;

namespace OrderPaymentSystem.Domain.Dto.OrderItem
{
    public record UpdateOrderItemDto
    {
        public int ProductId { get; set; }
        public int NewQuantity { get; set; }
    }
}
