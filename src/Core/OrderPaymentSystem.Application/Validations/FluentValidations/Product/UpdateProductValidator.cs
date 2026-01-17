using FluentValidation;
using OrderPaymentSystem.Application.DTOs.Product;
using OrderPaymentSystem.Domain.Resources;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.Product;

public class UpdateProductValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Имя товара не может быть пустым")
            .MaximumLength(100).WithMessage("Имя товара должно быть не длиннее 100 символов");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Описание товара не может быть пустым")
            .MaximumLength(100).WithMessage("Описание товара должно быть не длиннее 1000 символов");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage(ErrorMessage.ProductPricePositive);

        RuleFor(x => x.StockQuantity)
            .GreaterThan(0).WithMessage(ErrorMessage.StockQuantityPositive);
    }
}
