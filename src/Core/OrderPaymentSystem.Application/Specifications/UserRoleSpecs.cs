using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Shared.Specifications;

namespace OrderPaymentSystem.Application.Specifications;

internal static class UserRoleSpecs
{
    public static BaseSpecification<UserRole> ByUserIdRoleId(Guid userId, int roleId)
        => new(x => x.UserId == userId && x.RoleId == roleId);
}
