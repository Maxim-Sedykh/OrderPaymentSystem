using FluentValidation;
using OrderPaymentSystem.Domain.Dto.Payment;
using OrderPaymentSystem.Domain.Dto.UserRole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.UserRole
{
    public class DeleteUserRoleValidation : AbstractValidator<DeleteUserRoleDto>
    {
        public DeleteUserRoleValidation()
        {
            RuleFor(x => x.Login)
                .NotEmpty().WithMessage("Логин пользователя должен быть указан");
            RuleFor(x => x.RoleId)
                .NotEmpty().WithMessage("Идентификатор для роли должен быть указан");
        }
    }
}
