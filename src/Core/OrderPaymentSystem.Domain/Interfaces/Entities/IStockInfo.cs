using System;
using System.Collections.Generic;
using System.Text;

namespace OrderPaymentSystem.Domain.Interfaces.Entities;

public interface IStockInfo
{
    bool IsStockQuantityAvailable(int requestedQuantity);
}
