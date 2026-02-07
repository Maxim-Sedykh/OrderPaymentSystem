using FluentValidation;
using OrderPaymentSystem.Application.DTOs.Auth;
using OrderPaymentSystem.Application.Extensions;
using OrderPaymentSystem.Domain.Errors;
using static OrderPaymentSystem.Domain.Constants.ValidationConstants.User;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.Auth;

public class RegisterUserValidator : AbstractValidator<RegisterUserDto>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty()
            .WithError(DomainErrors.Validation.Required(nameof(RegisterUserDto.Login)))
            .MaximumLength(MaxLoginLength)
            .WithError(DomainErrors.Validation.TooLong(nameof(RegisterUserDto.Login), MaxLoginLength))
            .MinimumLength(MinLoginLength)
            .WithError(DomainErrors.Validation.TooShort(nameof(RegisterUserDto.Login), MinLoginLength));

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithError(DomainErrors.Validation.Required(nameof(RegisterUserDto.Password)))
            .MinimumLength(MinPasswordLength)
            .WithError(DomainErrors.Validation.TooShort(nameof(RegisterUserDto.Password), MinPasswordLength))
            .MaximumLength(MaxPasswordLength)
            .WithError(DomainErrors.Validation.TooShort(nameof(RegisterUserDto.Password), MinPasswordLength));

        RuleFor(x => x.PasswordConfirm)
            .NotEmpty()
            .WithError(DomainErrors.Validation.Required(nameof(RegisterUserDto.PasswordConfirm)))
            .Equal(x => x.Password)
            .WithError(DomainErrors.User.PasswordMismatch());
    }
}
