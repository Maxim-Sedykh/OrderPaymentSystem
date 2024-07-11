using FluentValidation;
using OrderPaymentSystem.Domain.Dto.Product;
using OrderPaymentSystem.Domain.Dto.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
