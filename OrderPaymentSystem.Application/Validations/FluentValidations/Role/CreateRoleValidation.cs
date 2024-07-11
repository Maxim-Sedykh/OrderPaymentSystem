using FluentValidation;
using OrderPaymentSystem.Domain.Dto.Role;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.Role
{
    public class CreateRoleValidation : AbstractValidator<CreateRoleDto>
    {
        public CreateRoleValidation()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Название роли не может быть пустым")
                .MaximumLength(50).WithMessage("Название роли должно быть не длиннее 50 символов");
        }
    }
}
