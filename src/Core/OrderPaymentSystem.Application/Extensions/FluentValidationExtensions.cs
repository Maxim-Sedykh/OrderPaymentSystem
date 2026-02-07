using FluentValidation;
using OrderPaymentSystem.Shared.Result;

namespace OrderPaymentSystem.Application.Extensions;

public static class FluentValidationExtensions
{
    public static IRuleBuilderOptions<T, TProperty> WithError<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule, Error error)
    {
        return rule
            .WithErrorCode(error.Code.ToString())
            .WithMessage(error.Message);
    }
}
