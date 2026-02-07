using FluentValidation;
using OrderPaymentSystem.Application.DTOs.Order;
using OrderPaymentSystem.Application.Extensions;
using OrderPaymentSystem.Domain.Errors;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.Order;

public class CreateOrderValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.OrderItems)
            .NotEmpty()
            .WithError(DomainErrors.Validation.Required(nameof(CreateOrderDto.OrderItems)));

        RuleFor(x => x.DeliveryAddress)
            .NotEmpty()
            .WithError(DomainErrors.Order.DeliveryAddressRequired());
    }
}
