using FluentValidation;
using OrderPaymentSystem.Domain.Dto.Role;
using OrderPaymentSystem.Domain.Dto.UserRole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.UserRole
{
    public class UserRoleValidation : AbstractValidator<UserRoleDto>
    {
        public UserRoleValidation()
        {
            RuleFor(x => x.Login)
                .NotEmpty().WithMessage("Логин должен быть указан");

            RuleFor(x => x.RoleName)
                .NotEmpty().WithMessage("Название роли не может быть пустым");
        }
    }
}
