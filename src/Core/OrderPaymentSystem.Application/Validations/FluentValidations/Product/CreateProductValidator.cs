using FluentValidation;
using OrderPaymentSystem.Application.DTOs.Product;
using OrderPaymentSystem.Application.Extensions;
using OrderPaymentSystem.Domain.Errors;
using static OrderPaymentSystem.Domain.Constants.ValidationConstants.Product;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.Product;

/// <summary>
/// Валидатор для <see cref="CreateProductDto"/>
/// </summary>
public class CreateProductValidator : AbstractValidator<CreateProductDto>
{
    /// <summary>
    /// Конструктор валидатора, создание правил для полей DTO <see cref="CreateProductDto"/>
    /// </summary>
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithError(DomainErrors.Validation.Required(nameof(CreateProductDto.Name)))
            .MinimumLength(MinNameLength)
            .WithError(DomainErrors.Validation.TooShort(nameof(CreateProductDto.Name), MinNameLength))
            .MaximumLength(MaxNameLength)
            .WithError(DomainErrors.Validation.TooLong(nameof(CreateProductDto.Name), MaxNameLength));

        RuleFor(x => x.Description)
            .MaximumLength(MaxDescriptionLength)
            .WithError(DomainErrors.Validation.TooLong(nameof(CreateProductDto.Description), MaxDescriptionLength));

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithError(DomainErrors.Product.PricePositive());

        RuleFor(x => x.StockQuantity)
            .GreaterThan(0)
            .WithError(DomainErrors.General.QuantityPositive());
    }
}
