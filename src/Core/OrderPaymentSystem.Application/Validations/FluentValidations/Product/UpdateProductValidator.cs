using FluentValidation;
using OrderPaymentSystem.Application.DTOs.Product;
using OrderPaymentSystem.Application.Extensions;
using OrderPaymentSystem.Domain.Errors;
using static OrderPaymentSystem.Domain.Constants.ValidationConstants.Product;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.Product;

public class UpdateProductValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithError(DomainErrors.Validation.Required(nameof(UpdateProductDto.Name)))
            .MaximumLength(MaxNameLength)
            .WithError(DomainErrors.Validation.TooLong(nameof(UpdateProductDto.Name), MaxNameLength));

        RuleFor(x => x.Description)
            .MaximumLength(MaxDescriptionLength)
            .WithError(DomainErrors.Validation.TooLong(nameof(UpdateProductDto.Description), MaxDescriptionLength));

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithError(DomainErrors.Product.PricePositive());

        RuleFor(x => x.StockQuantity)
            .GreaterThan(0)
            .WithError(DomainErrors.General.QuantityPositive());
    }
}
