using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Shared.Specifications;

namespace OrderPaymentSystem.Application.Specifications;

public static class RoleSpecs
{
    public static BaseSpecification<Role> ById(int id)
        => new(x => x.Id == id);

    public static BaseSpecification<Role> ByName(string name)
        => new(x => x.Name == name);

    public static BaseSpecification<Role> ByUserId(Guid userId)
    {
        return new BaseSpecification<Role>(role =>
            role.Users.Any(ur => ur.Id == userId));
    }
}
