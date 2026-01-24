using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Shared.Specifications;

namespace OrderPaymentSystem.Application.Specifications;

public static class ProductSpecs
{
    public static BaseSpecification<Product> ById(long id)
        => new(x => x.Id == id);

    public static BaseSpecification<Product> ByName(string name)
        => new(x => x.Name == name);

    public static BaseSpecification<Product> ByIdNoTracking(long id)
    {
        var spec = new BaseSpecification<Product>(x => x.Id == id);

        spec.ApplyAsNoTracking();

        return spec;
    }
}
