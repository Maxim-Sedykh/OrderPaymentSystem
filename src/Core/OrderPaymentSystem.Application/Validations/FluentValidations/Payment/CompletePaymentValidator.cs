using FluentValidation;
using OrderPaymentSystem.Application.DTOs.Payment;
using OrderPaymentSystem.Application.Extensions;
using OrderPaymentSystem.Domain.Errors;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.Payment;

/// <summary>
/// Валидатор для <see cref="CompletePaymentDto"/>
/// </summary>
public class CompletePaymentValidator : AbstractValidator<CompletePaymentDto>
{
    /// <summary>
    /// Конструктор валидатора, создание правил для полей DTO <see cref="CompletePaymentDto"/>
    /// </summary>
    public CompletePaymentValidator()
    {
        RuleFor(x => x.AmountPaid)
            .NotEmpty()
            .WithError(DomainErrors.Payment.AmountPositive())
            .GreaterThan(0)
            .WithError(DomainErrors.Payment.AmountPositive());

        RuleFor(x => x.CashChange)
            .GreaterThanOrEqualTo(0)
            .WithError(DomainErrors.Payment.CashChangeMismatch());
    }
}