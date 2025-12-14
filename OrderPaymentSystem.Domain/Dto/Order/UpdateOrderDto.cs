namespace OrderPaymentSystem.Domain.Dto.Order;

public record UpdateOrderDto(
        int ProductId,
        int ProductCount
    );
