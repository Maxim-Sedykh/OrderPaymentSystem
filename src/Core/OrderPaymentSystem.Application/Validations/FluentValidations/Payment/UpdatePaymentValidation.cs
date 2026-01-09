using FluentValidation;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.Payment;

public class UpdatePaymentValidator : AbstractValidator<UpdatePaymentDto>
{
    public UpdatePaymentValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Идентификатор платежа должен быть указан");
        RuleFor(x => x.AmountOfPayment).GreaterThan(0).WithMessage("Оплата должна быть больше нуля");
    }
}
