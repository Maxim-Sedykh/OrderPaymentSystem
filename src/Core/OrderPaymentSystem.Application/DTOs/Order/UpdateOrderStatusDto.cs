using OrderPaymentSystem.Domain.Enum;

namespace OrderPaymentSystem.Application.DTOs.Order;

public record UpdateOrderStatusDto
{
    public OrderStatus NewStatus { get; set; }
}
