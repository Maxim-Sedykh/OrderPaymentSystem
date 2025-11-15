using FluentValidation;
using OrderPaymentSystem.Domain.Dto.Auth;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.Auth;

public class LoginUserValidator : AbstractValidator<LoginUserDto>
{
    public LoginUserValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty().WithMessage("Логин пользователя должен быть указан");
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль пользователя должен быть указан");
    }
}
