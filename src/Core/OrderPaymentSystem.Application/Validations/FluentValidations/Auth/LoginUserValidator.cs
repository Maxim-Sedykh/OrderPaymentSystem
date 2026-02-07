using FluentValidation;
using OrderPaymentSystem.Application.DTOs.Auth;
using OrderPaymentSystem.Application.Extensions;
using OrderPaymentSystem.Domain.Errors;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.Auth;

public class LoginUserValidator : AbstractValidator<LoginUserDto>
{
    public LoginUserValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty()
            .WithError(DomainErrors.Validation.Required(nameof(LoginUserDto.Login)));

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithError(DomainErrors.Validation.Required(nameof(LoginUserDto.Password)));
    }
}
