namespace OrderPaymentSystem.Application.DTOs.Product;

public record CreateProductDto(string Name, string Description, decimal Price, int StockQuantity);
