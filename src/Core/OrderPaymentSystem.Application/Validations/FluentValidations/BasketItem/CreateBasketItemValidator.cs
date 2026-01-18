using FluentValidation;
using OrderPaymentSystem.Application.DTOs.Basket;
using OrderPaymentSystem.Domain.Resources;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.BasketItem;

public class CreateBasketItemValidator : AbstractValidator<CreateBasketItemDto>
{
    public CreateBasketItemValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage(ErrorMessage.InvalidProductId);

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage(ErrorMessage.QuantityPositive);
    }
}
