using FluentValidation;
using OrderPaymentSystem.Application.DTOs.Payment;
using OrderPaymentSystem.Domain.Resources;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.Payment;

public class CreatePaymentValidator : AbstractValidator<CreatePaymentDto>
{
    public CreatePaymentValidator()
    {
        RuleFor(x => x.AmountPayed)
            .GreaterThan(0)
            .WithMessage(ErrorMessage.PaymentAmountPositive);

        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage(ErrorMessage.InvalidOrderId);
    }
}
