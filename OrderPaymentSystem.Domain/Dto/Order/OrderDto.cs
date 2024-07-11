namespace OrderPaymentSystem.Domain.Dto.Order
{
    public record OrderDto(
            long Id,
            long UserId,
            long BasketId,
            long ProductId,
            int ProductCount,
            string CreatedAt
        );
}
