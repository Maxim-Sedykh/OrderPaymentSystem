using System;
using System.Collections.Generic;
using System.Text;

namespace OrderPaymentSystem.Domain.Dto.Basket
{
    public class BasketItemDto
    {
        public long Id { get; set; }
        public Guid UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
