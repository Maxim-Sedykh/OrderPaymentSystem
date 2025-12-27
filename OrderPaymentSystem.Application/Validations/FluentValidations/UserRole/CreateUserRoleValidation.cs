using FluentValidation;
using OrderPaymentSystem.Domain.Dto.UserRole;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.UserRole
{
    public class CreateUserRoleValidation : AbstractValidator<CreateUserRoleDto>
    {
        public CreateUserRoleValidation()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("Идентификатор пользователя был пустой");
            RuleFor(x => x.RoleId)
                .NotEmpty().WithMessage("Идентификатор для роли должен быть указан");
        }
    }
}
