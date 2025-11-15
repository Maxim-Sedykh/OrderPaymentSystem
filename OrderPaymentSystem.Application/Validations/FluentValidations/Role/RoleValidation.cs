using FluentValidation;
using OrderPaymentSystem.Domain.Dto.Role;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.Role;

public class RoleValidation : AbstractValidator<RoleDto>
{
    public RoleValidation()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Идентификатор роли должен быть указан");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Название роли не может быть пустым")
            .MaximumLength(50).WithMessage("Название роли должно быть не длиннее 50 символов");
    }
}
