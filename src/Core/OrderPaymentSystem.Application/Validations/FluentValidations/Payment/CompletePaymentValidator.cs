using FluentValidation;
using OrderPaymentSystem.Application.DTOs.Payment;
using OrderPaymentSystem.Application.Extensions;
using OrderPaymentSystem.Domain.Errors;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.Payment;

public class CompletePaymentValidator : AbstractValidator<CompletePaymentDto>
{
    public CompletePaymentValidator()
    {
        RuleFor(x => x.AmountPaid)
            .NotEmpty()
            .WithError(DomainErrors.Payment.AmountPositive())
            .GreaterThan(0)
            .WithError(DomainErrors.Payment.AmountPositive());

        RuleFor(x => x.CashChange)
            .GreaterThanOrEqualTo(0)
            .WithError(DomainErrors.Payment.CashChangeMismatch());
    }
}