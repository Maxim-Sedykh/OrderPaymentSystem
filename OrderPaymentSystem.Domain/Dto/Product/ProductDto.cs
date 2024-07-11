namespace OrderPaymentSystem.Domain.Dto.Product
{
    public record ProductDto(long Id, string ProductName, string Description, decimal Cost, string CreatedAt);
}
