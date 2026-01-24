using FluentValidation;
using OrderPaymentSystem.Application.DTOs.OrderItem;
using OrderPaymentSystem.Domain.Resources;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.Order;

public class UpdateOrderItemValidator : AbstractValidator<UpdateOrderItemDto>
{
    public UpdateOrderItemValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage(ErrorMessage.InvalidProductId);

        RuleFor(x => x.NewQuantity)
            .GreaterThan(0)
            .WithMessage(ErrorMessage.QuantityPositive);
    }
}
