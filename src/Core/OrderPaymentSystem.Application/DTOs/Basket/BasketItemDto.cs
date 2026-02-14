namespace OrderPaymentSystem.Application.DTOs.Basket;

public record BasketItemDto(long Id, Guid UserId, int ProductId, int Quantity);
