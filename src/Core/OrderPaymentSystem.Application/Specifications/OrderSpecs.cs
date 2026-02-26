using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Shared.Specifications;

namespace OrderPaymentSystem.Application.Specifications;

public static class OrderSpecs
{
    public static BaseSpecification<Order> WithItems(this BaseSpecification<Order> spec)
        => spec.AddInclude(q => q.Include(o => o.Items));

    public static BaseSpecification<Order> ForShip(this BaseSpecification<Order> spec)
        => spec.AddInclude(q => q.Include(o => o.Items).ThenInclude(x => x.Product).Include(x => x.Payment));

    public static BaseSpecification<Order> ById(long id)
        => new(x => x.Id == id);

    public static BaseSpecification<Order> ByStatus(OrderStatus status)
        => new(x => x.Status == status);

    public static BaseSpecification<Order> CreatedBefore(this BaseSpecification<Order> spec, DateTime thresholdDate)
        => spec.And(x => x.CreatedAt <= thresholdDate);

    public static BaseSpecification<Order> ByUserIdNoTracking(Guid id)
    {
        var spec = new BaseSpecification<Order>(x => x.UserId == id);

        spec.ApplyAsNoTracking();

        return spec;
    }
}
