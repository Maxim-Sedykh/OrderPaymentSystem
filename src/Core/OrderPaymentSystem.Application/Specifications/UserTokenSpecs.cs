using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Shared.Specifications;

namespace OrderPaymentSystem.Application.Specifications
{
    internal static class UserTokenSpecs
    {
        public static BaseSpecification<UserToken> ExpiredBefore(this BaseSpecification<UserToken> spec, DateTime thresholdDate)
            => spec.And(x => x.RefreshTokenExpireTime <= thresholdDate);
    }
}
