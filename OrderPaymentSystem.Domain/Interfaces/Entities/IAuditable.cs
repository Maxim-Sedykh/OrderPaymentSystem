namespace OrderPaymentSystem.Domain.Interfaces.Entities;

/// <summary>
/// Интерфейс для маркировки сущностей, изменение и создание которых нужно отслеживать по времени
/// </summary>
public interface IAuditable
{
    /// <summary>
    /// Дата создания сущности
    /// </summary>
    public DateTime CreatedAt { get; }

    /// <summary>
    /// Дата последнего обновления сущности
    /// </summary>
    public DateTime? UpdatedAt { get; }
}
