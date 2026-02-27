namespace OrderPaymentSystem.Domain.Abstract.Interfaces.Entities;

/// <summary>
/// Информация по количеству товара на складе
/// </summary>
public interface IStockInfo
{
    /// <summary>
    /// Доступно ли запрашиваемое количество товара.
    /// Хватает ли его на складе.
    /// </summary>
    /// <param name="requestedQuantity">Запрашиваемое количество.</param>
    /// <returns>True - запрошенное количество товара присутствует на складе.</returns>
    bool IsStockQuantityAvailable(int requestedQuantity);
}
