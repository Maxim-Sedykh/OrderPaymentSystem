using FluentValidation;
using OrderPaymentSystem.Application.DTOs.Role;
using OrderPaymentSystem.Domain.Resources;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.Role;

public class CreateRoleValidator : AbstractValidator<CreateRoleDto>
{
    public CreateRoleValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ErrorMessage.RoleNameCannotBeEmpty)
            .MaximumLength(50).WithMessage("Название роли должно быть не длиннее 50 символов");
    }
}
