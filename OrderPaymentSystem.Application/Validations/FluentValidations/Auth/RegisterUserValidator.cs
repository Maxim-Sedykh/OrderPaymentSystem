using FluentValidation;
using OrderPaymentSystem.Domain.Dto.Auth;
using OrderPaymentSystem.Domain.Dto.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.Auth
{
    public class RegisterUserValidation : AbstractValidator<RegisterUserDto>
    {
        public RegisterUserValidation()
        {
            RuleFor(x => x.Login)
                .NotEmpty().WithMessage("Логин пользователя должен быть указан")
                .MaximumLength(50).WithMessage("Логин должен быть меньше 50 символов")
                .MinimumLength(5).WithMessage("Логин должен быть больше 5 символов");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Пароль пользователя должен быть указан")
                .MaximumLength(50).WithMessage("Пароль должен быть меньше 50 символов")
                .MinimumLength(5).WithMessage("Пароль должен быть больше 5 символов");
            RuleFor(x => x.PasswordConfirm)
                .NotEmpty().WithMessage("Подтверждение пароля должно быть указано");
        }
    }
}
