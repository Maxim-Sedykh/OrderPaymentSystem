using FluentValidation;
using OrderPaymentSystem.Domain.Dto.UserRole;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.UserRole
{
    public class UpdateUserRoleValidation : AbstractValidator<UpdateUserRoleDto>
    {
        public UpdateUserRoleValidation()
        {
            RuleFor(x => x.Login)
                .NotEmpty().WithMessage("Логин пользователя должен быть указан");
            RuleFor(x => x.ToRoleId)
                .NotEmpty().WithMessage("Идентификатор для начальной роли должен быть указан");
            RuleFor(x => x.FromRoleId)
                .NotEmpty().WithMessage("Идентификатор для конечной роли должен быть указан");
        }
    }
}
