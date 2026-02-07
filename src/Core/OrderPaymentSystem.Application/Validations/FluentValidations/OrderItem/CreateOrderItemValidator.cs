using FluentValidation;
using OrderPaymentSystem.Application.DTOs.OrderItem;
using OrderPaymentSystem.Application.Extensions;
using OrderPaymentSystem.Domain.Errors;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.OrderItem;

public class CreateOrderItemValidator : AbstractValidator<CreateOrderItemDto>
{
    public CreateOrderItemValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithError(DomainErrors.Validation.Required(nameof(CreateOrderItemDto.ProductId)));

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithError(DomainErrors.General.QuantityPositive());
    }
}
