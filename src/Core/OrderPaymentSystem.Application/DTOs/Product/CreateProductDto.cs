namespace OrderPaymentSystem.Application.DTOs.Product;

/// <summary>
/// Модель для создания товара
/// </summary>
/// <param name="Name">Название товара</param>
/// <param name="Description">Описание товара</param>
/// <param name="Price">Цена за единицу товара</param>
/// <param name="StockQuantity">Количество товара на складе</param>
public record CreateProductDto(string Name, string Description, decimal Price, int StockQuantity);
