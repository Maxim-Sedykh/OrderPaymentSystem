namespace OrderPaymentSystem.Application.DTOs.Product;

public record ProductDto(int Id, string Name, string Description, decimal Price, int StockQuantity, DateTime CreatedAt);