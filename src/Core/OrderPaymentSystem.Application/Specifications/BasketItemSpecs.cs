using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Shared.Specifications;

namespace OrderPaymentSystem.Application.Specifications;

internal static class BasketItemSpecs
{
    public static BaseSpecification<BasketItem> WithProduct(this BaseSpecification<BasketItem> spec)
        => spec.AddInclude(q => q.Include(o => o.Product));

    public static BaseSpecification<BasketItem> ById(long id)
        => new(x => x.Id == id);

    public static BaseSpecification<BasketItem> ByUserId(Guid id)
        => new(x => x.UserId == id);
}
