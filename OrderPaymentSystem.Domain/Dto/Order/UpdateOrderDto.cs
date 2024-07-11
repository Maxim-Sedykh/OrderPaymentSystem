namespace OrderPaymentSystem.Domain.Dto.Order
{
    public record UpdateOrderDto(
            long Id,
            int ProductId,
            int ProductCount
        );
}
