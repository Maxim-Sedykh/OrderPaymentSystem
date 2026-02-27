using FluentValidation;
using OrderPaymentSystem.Application.DTOs.Order;
using OrderPaymentSystem.Application.Extensions;
using OrderPaymentSystem.Domain.Errors;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.Order;

/// <summary>
/// Валидатор для <see cref="CreateOrderDto"/>
/// </summary>
public class CreateOrderValidator : AbstractValidator<CreateOrderDto>
{
    /// <summary>
    /// Конструктор валидатора, создание правил для полей DTO <see cref="CreateOrderDto"/>
    /// </summary>
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
