using OrderPaymentSystem.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderPaymentSystem.Application.DTOs.Order;

public record UpdateOrderStatusDto
{
    public OrderStatus NewStatus { get; set; }
}
