using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Domain.Dto.Order
{
    /// <summary>
    /// Модель, предназначенная для методов действий с заказами (создание, удаление)
    /// </summary>
    /// <param name="ProductId"></param>
    /// <param name="ProductCount"></param>
    public record CreateOrderDto
    (
        string ProductId,
        int ProductCount
    );
}
