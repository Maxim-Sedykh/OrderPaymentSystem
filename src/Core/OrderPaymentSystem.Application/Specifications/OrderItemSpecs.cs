using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Shared.Specifications;

namespace OrderPaymentSystem.Application.Specifications
{
    public static class OrderItemSpecs
    {
        public static BaseSpecification<OrderItem> ById(long id)
            => new(x => x.Id == id);

        public static BaseSpecification<OrderItem> ByOrderId(long orderId)
            => new(x => x.OrderId == orderId);

        public static BaseSpecification<OrderItem> WithProduct(this BaseSpecification<OrderItem> spec)
            => spec.AddInclude(q => q.Include(o => o.Product));
    }
}
