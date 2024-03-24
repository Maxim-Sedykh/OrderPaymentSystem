using FluentValidation;
using OrderPaymentSystem.Domain.Dto.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.Product
{
    public class UpdateProductValidator : AbstractValidator<UpdateProductDto>
    {
        public UpdateProductValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Идентификатор товара не должен быть пустым");
            RuleFor(x => x.ProductName)
                .NotEmpty().WithMessage("Имя товара не может быть пустым")
                .MaximumLength(100).WithMessage("Имя товара должно быть не длиннее 100 символов");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Описание товара не может быть пустым")
                .MaximumLength(100).WithMessage("Описание товара должно быть не длиннее 1000 символов");
        }
    }
}
