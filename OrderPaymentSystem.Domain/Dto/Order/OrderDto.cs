using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderPaymentSystem.Domain.Dto.Order
{
    public class OrderDto
    {
        public long Id { get; set; }
        public Guid UserId { get; set; }
        public long? PaymentId { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public Address DeliveryAddress { get; set; }
        public List<OrderItemDto> Items { get; set; }
    }
}
