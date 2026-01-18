using FluentValidation;
using OrderPaymentSystem.Application.DTOs;
using OrderPaymentSystem.Domain.Resources;

namespace OrderPaymentSystem.Application.Validations.FluentValidations;

public class UpdateQuantityValidator : AbstractValidator<UpdateQuantityDto>
{
    public UpdateQuantityValidator()
    {
        RuleFor(x => x.NewQuantity)
            .GreaterThan(0).WithMessage(ErrorMessage.QuantityPositive);
    }
}
