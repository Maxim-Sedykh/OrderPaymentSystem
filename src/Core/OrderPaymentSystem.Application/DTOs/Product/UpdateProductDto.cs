namespace OrderPaymentSystem.Application.DTOs.Product;

/// <summary>
/// Модель для обновления данных товара
/// </summary>
public class UpdateProductDto
{
    /// <summary>
    /// Название товара
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Описание
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Цена за единицу товара
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Кол-во товара на складе
    /// </summary>
    public int StockQuantity { get; set; }
}
