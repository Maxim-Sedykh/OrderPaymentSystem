using FluentValidation;
using OrderPaymentSystem.Domain.Dto.Payment;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.Payment
{
    public class CreatePaymentValidator : AbstractValidator<CreatePaymentDto>
    {
        public CreatePaymentValidator()
        {
            RuleFor(x => x.BasketId)
                .NotEmpty().WithMessage("Идентификатор для корзины должен быть указан");
            RuleFor(x => x.AmountOfPayment).GreaterThan(0).WithMessage("Оплата должна быть больше нуля");
        }
    }
}
