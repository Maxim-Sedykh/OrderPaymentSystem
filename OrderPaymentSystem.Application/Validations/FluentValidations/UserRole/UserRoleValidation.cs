using FluentValidation;
using OrderPaymentSystem.Domain.Dto.UserRole;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.UserRole;

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
