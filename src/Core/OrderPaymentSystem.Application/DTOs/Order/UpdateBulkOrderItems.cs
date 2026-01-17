using OrderPaymentSystem.Application.DTOs.OrderItem;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderPaymentSystem.Application.DTOs.Order
{
    public class UpdateBulkOrderItemsDto
    {
        public List<UpdateOrderItemDto> Items { get; set; } = new();
    }
}
