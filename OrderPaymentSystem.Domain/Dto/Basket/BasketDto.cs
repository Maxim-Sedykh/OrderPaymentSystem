namespace OrderPaymentSystem.Domain.Dto.Basket;

public class BasketDto
{
    public long Id { get; set; }

    public Guid UserId { get; set; }

    public string CreatedAt { get; set; }

    public decimal CostOfAllOrders { get; set; }
}
