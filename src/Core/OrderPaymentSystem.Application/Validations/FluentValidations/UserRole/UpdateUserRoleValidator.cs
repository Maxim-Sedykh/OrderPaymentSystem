using FluentValidation;
using OrderPaymentSystem.Application.DTOs.UserRole;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.UserRole;

public class UpdateUserRoleValidator : AbstractValidator<UpdateUserRoleDto>
{
    public UpdateUserRoleValidator()
    {
        RuleFor(x => x.ToRoleId)
            .NotEmpty().WithMessage("Идентификатор для начальной роли должен быть указан");
        RuleFor(x => x.FromRoleId)
            .NotEmpty().WithMessage("Идентификатор для конечной роли должен быть указан");
    }
}
