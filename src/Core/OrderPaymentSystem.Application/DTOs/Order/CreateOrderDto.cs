using OrderPaymentSystem.Application.DTOs.OrderItem;
using OrderPaymentSystem.Domain.ValueObjects;

namespace OrderPaymentSystem.Application.DTOs.Order;

public record CreateOrderDto
{
    public List<CreateOrderItemDto> OrderItems { get; set; }
    public Address DeliveryAddress { get; set; }
}
