using FluentValidation;
using OrderPaymentSystem.Application.DTOs.Payment;
using OrderPaymentSystem.Domain.Resources;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.Payment;

public class CompletePaymentValidator : AbstractValidator<CompletePaymentDto>
{
    public CompletePaymentValidator()
    {
        RuleFor(x => x.AmountPaid)
            .NotEmpty()
            .WithMessage(ErrorMessage.PaymentAmountPositive)
            .GreaterThan(0)
            .WithMessage(ErrorMessage.PaymentAmountPositive);

        RuleFor(x => x.CashChange)
            .GreaterThanOrEqualTo(0)
            .WithMessage(ErrorMessage.PaymentCashChangeMismatch);
    }
}