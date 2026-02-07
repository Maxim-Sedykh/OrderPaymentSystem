namespace OrderPaymentSystem.Domain.Abstract.Interfaces.Entities;

public interface IStockInfo
{
    bool IsStockQuantityAvailable(int requestedQuantity);
}
