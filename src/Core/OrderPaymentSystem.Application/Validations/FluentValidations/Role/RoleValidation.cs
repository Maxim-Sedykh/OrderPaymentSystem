using FluentValidation;
using OrderPaymentSystem.Application.DTOs.Role;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.Role;

public class UpdateRoleValidator : AbstractValidator<UpdateRoleDto>
{
    public UpdateRoleValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Название роли не может быть пустым")
            .MaximumLength(50).WithMessage("Название роли должно быть не длиннее 50 символов");
    }
}
