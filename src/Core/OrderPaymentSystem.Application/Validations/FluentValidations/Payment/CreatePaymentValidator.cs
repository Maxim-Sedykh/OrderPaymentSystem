using FluentValidation;
using OrderPaymentSystem.Application.DTOs.Payment;
using OrderPaymentSystem.Application.Extensions;
using OrderPaymentSystem.Domain.Errors;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.Payment;

public class CreatePaymentValidator : AbstractValidator<CreatePaymentDto>
{
    public CreatePaymentValidator()
    {
        RuleFor(x => x.AmountPayed)
            .GreaterThan(0)
            .WithError(DomainErrors.Payment.AmountPositive());

        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithError(DomainErrors.Validation.Required(nameof(CreatePaymentDto.OrderId)));
    }
}
