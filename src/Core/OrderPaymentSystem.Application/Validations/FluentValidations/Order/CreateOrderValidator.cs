using FluentValidation;
using OrderPaymentSystem.Application.DTOs.Order;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.Order;

public class CreateOrderValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.OrderItems)
            .NotEmpty().WithMessage("Идентификатор пользователя должен быть указан");
        RuleFor(x => x.DeliveryAddress)
            .NotEmpty().WithMessage("Идентификатор товара должен быть указан");
    }
}
