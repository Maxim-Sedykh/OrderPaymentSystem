using FluentValidation;
using OrderPaymentSystem.Application.DTOs.UserRole;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.UserRole;

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
