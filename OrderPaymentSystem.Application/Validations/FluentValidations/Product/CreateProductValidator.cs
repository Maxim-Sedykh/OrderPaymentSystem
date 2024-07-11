using FluentValidation;
using OrderPaymentSystem.Domain.Dto.Product;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.Product
{
    public class CreateProductValidator : AbstractValidator<CreateProductDto>
    {
        public CreateProductValidator()
        {
            RuleFor(x => x.ProductName)
                .NotEmpty().WithMessage("Имя товара не может быть пустым")
                .MinimumLength(3)
                .MaximumLength(100).WithMessage("Имя товара должно быть не длиннее 100 символов");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Описание товара не может быть пустым")
                .MaximumLength(100).WithMessage("Описание товара должно быть не длиннее 1000 символов");

            RuleFor(x => x.Cost)
                .GreaterThan(0).WithMessage("Cтоимость товара должно быть больше 0")
                .NotEmpty().WithMessage("Cтоимость товара должно быть указано");
        }
    }
}
