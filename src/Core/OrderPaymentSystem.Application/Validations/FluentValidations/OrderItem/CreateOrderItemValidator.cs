using FluentValidation;
using OrderPaymentSystem.Application.DTOs.OrderItem;
using OrderPaymentSystem.Domain.Resources;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.OrderItem;

public class CreateOrderItemValidator : AbstractValidator<CreateOrderItemDto>
{
    public CreateOrderItemValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage(ErrorMessage.InvalidOrderId);

        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage(ErrorMessage.InvalidProductId);

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage(ErrorMessage.QuantityPositive);
    }
}
