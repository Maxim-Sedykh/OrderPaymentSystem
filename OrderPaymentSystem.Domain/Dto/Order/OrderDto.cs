namespace OrderPaymentSystem.Domain.Dto.Order;

public record OrderDto(
        long Id,
        Guid UserId,
        long BasketId,
        long ProductId,
        int ProductCount,
        string CreatedAt
    );
