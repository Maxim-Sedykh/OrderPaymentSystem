using FluentValidation;
using OrderPaymentSystem.Application.DTOs.OrderItem;
using OrderPaymentSystem.Application.Extensions;
using OrderPaymentSystem.Domain.Errors;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.Order;

public class UpdateOrderItemValidator : AbstractValidator<UpdateOrderItemDto>
{
    public UpdateOrderItemValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithError(DomainErrors.Validation.Required(nameof(UpdateOrderItemDto.ProductId)));

        RuleFor(x => x.NewQuantity)
            .GreaterThan(0)
            .WithError(DomainErrors.General.QuantityPositive());
    }
}
