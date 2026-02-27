using FluentValidation;
using OrderPaymentSystem.Application.DTOs.Basket;
using OrderPaymentSystem.Application.Extensions;
using OrderPaymentSystem.Domain.Errors;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.BasketItem;

/// <summary>
/// Валидатор для <see cref="CreateBasketItemDto"/>
/// </summary>
public class CreateBasketItemValidator : AbstractValidator<CreateBasketItemDto>
{
    /// <summary>
    /// Конструктор валидатора, создание правил для полей DTO <see cref="CreateBasketItemDto"/>
    /// </summary>
    public CreateBasketItemValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithError(DomainErrors.Validation.Required(nameof(CreateBasketItemDto.ProductId)));

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithError(DomainErrors.General.QuantityPositive());
    }
}
