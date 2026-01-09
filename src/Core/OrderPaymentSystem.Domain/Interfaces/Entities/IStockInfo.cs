namespace OrderPaymentSystem.Domain.Interfaces.Entities;

public interface IStockInfo
{
    bool IsStockQuantityAvailable(int requestedQuantity);
}
