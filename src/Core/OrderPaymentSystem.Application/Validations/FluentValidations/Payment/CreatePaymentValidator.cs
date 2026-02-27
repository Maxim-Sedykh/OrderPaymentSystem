using FluentValidation;
using OrderPaymentSystem.Application.DTOs.Payment;
using OrderPaymentSystem.Application.Extensions;
using OrderPaymentSystem.Domain.Errors;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.Payment;

/// <summary>
/// Валидатор для <see cref="CreatePaymentDto"/>
/// </summary>
public class CreatePaymentValidator : AbstractValidator<CreatePaymentDto>
{
    /// <summary>
    /// Конструктор валидатора, создание правил для полей DTO <see cref="CompletePaymentDto"/>
    /// </summary>
    public CreatePaymentValidator()
    {
        RuleFor(x => x.AmountPaid)
            .GreaterThan(0)
            .WithError(DomainErrors.Payment.AmountPositive());

        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithError(DomainErrors.Validation.Required(nameof(CreatePaymentDto.OrderId)));
    }
}
