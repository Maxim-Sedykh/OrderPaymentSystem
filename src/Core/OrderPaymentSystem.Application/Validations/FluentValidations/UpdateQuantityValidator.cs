using FluentValidation;
using OrderPaymentSystem.Application.DTOs;
using OrderPaymentSystem.Application.Extensions;
using OrderPaymentSystem.Domain.Errors;

namespace OrderPaymentSystem.Application.Validations.FluentValidations;

/// <summary>
/// Валидатор для <see cref="UpdateQuantityDto"/>
/// </summary>
public class UpdateQuantityValidator : AbstractValidator<UpdateQuantityDto>
{
    /// <summary>
    /// Конструктор валидатора, создание правил для полей DTO <see cref="UpdateQuantityDto"/>
    /// </summary>
    public UpdateQuantityValidator()
    {
        RuleFor(x => x.NewQuantity)
            .GreaterThan(0)
            .WithError(DomainErrors.General.QuantityPositive());
    }
}
