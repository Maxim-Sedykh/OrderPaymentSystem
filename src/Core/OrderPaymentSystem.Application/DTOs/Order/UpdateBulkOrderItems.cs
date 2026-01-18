using OrderPaymentSystem.Application.DTOs.OrderItem;

namespace OrderPaymentSystem.Application.DTOs.Order;

public class UpdateBulkOrderItemsDto
{
    public List<UpdateOrderItemDto> Items { get; set; } = new();
}
