using FluentValidation;
using OrderPaymentSystem.Domain.Dto.Order;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.Order;

public class CreateOrderValidation : AbstractValidator<CreateOrderDto>
{
    public CreateOrderValidation()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Идентификатор пользователя должен быть указан");
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Идентификатор товара должен быть указан");
        RuleFor(x => x.ProductCount)
            .GreaterThan(0).WithMessage("Количество товара должно быть больше 0")
            .NotEmpty().WithMessage("Количество товара должно быть указано");
    }
}
