namespace OrderPaymentSystem.Application.DTOs.Product;

public record ProductDto(long Id, string Name, string Description, decimal Price, int StockQuantity, DateTime CreatedAt);