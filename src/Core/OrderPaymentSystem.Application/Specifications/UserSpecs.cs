using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Shared.Specifications;

namespace OrderPaymentSystem.Application.Specifications;

public static class UserSpecs
{
    public static BaseSpecification<User> WithRoles(this BaseSpecification<User> spec)
        => spec.AddInclude(u => u.Include(u => u.Roles));

    public static BaseSpecification<User> WithToken(this BaseSpecification<User> spec)
        => spec.AddInclude(u => u.Include(u => u.UserToken));

    public static BaseSpecification<User> ForAuth(this BaseSpecification<User> spec)
        => spec.AddInclude(u => u.Include(u => u.UserToken).Include(u => u.Roles));

    public static BaseSpecification<User> ById(Guid id)
        => new(x => x.Id == id);

    public static BaseSpecification<User> ByLogin(string login)
        => new(x => x.Login == login);
}
