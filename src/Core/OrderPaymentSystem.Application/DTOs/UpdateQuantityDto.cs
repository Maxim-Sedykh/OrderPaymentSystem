namespace OrderPaymentSystem.Application.DTOs;

/// <summary>
/// Модель данных для обновления количества
/// </summary>
public record UpdateQuantityDto
{
    /// <summary>
    /// Новое количество
    /// </summary>
    public int NewQuantity { get; set; }
}
