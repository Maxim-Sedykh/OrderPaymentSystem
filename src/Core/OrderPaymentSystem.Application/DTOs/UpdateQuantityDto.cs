namespace OrderPaymentSystem.Application.DTOs;

/// <summary>
/// Модель данных для обновления количества
/// </summary>
public sealed record UpdateQuantityDto
{
    /// <summary>
    /// Новое количество
    /// </summary>
    public int NewQuantity { get; set; }
}
