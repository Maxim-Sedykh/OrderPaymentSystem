using FluentValidation;
using OrderPaymentSystem.Application.DTOs.OrderItem;
using OrderPaymentSystem.Application.Extensions;
using OrderPaymentSystem.Domain.Errors;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.OrderItem;

/// <summary>
/// Валидатор для <see cref="CreateOrderItemDto"/>
/// </summary>
public class CreateOrderItemValidator : AbstractValidator<CreateOrderItemDto>
{
    /// <summary>
    /// Конструктор валидатора, создание правил для полей DTO <see cref="CreateOrderItemDto"/>
    /// </summary>
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
