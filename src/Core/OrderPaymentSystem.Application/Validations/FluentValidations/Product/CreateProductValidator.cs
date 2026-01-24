using FluentValidation;
using OrderPaymentSystem.Application.DTOs.Product;
using OrderPaymentSystem.Domain.Resources;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.Product;

public class CreateProductValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ErrorMessage.ProductNameEmpty)
            .MinimumLength(3).WithMessage("Name of product no less than 100 symbols")
            .MaximumLength(100).WithMessage("Name of product no longer 100 symbols");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description of product must be not empty")
            .MaximumLength(1000).WithMessage("Description of product must be no longer than 1000 symbols");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage(ErrorMessage.ProductPricePositive);

        RuleFor(x => x.StockQuantity)
            .GreaterThan(0).WithMessage(ErrorMessage.StockQuantityPositive);
    }
}
