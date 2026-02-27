namespace OrderPaymentSystem.Application.DTOs.Product;

/// <summary>
/// Модель данных для представления товара
/// </summary>
/// <param name="Id">Id товара</param>
/// <param name="Name">Название</param>
/// <param name="Description">Описание</param>
/// <param name="Price">Цена за единицу товара</param>
/// <param name="StockQuantity">Количество товара на складе</param>
/// <param name="CreatedAt">Создан в</param>
public record ProductDto(int Id, string Name, string Description, decimal Price, int StockQuantity, DateTime CreatedAt);