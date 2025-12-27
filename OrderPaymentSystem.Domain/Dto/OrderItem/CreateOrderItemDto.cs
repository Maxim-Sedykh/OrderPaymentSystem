using System;
using System.Collections.Generic;
using System.Text;

namespace OrderPaymentSystem.Domain.Dto.OrderItem
{
    public class CreateOrderItemDto
    {
        public long OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
