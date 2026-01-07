namespace OrderPaymentSystem.Application.DTOs.Product;

//public record ProductDto(long Id, string ProductName, string Description, decimal Cost, string CreatedAt);

public class ProductDto
{
    public int Id { get; set; }

    public string ProductName { get; set; }

    public string Description { get; set; }

    public decimal Cost { get; set; }

    public string CreatedAt { get; set;}
}
