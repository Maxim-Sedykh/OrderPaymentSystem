using FluentValidation;
using OrderPaymentSystem.Application.DTOs.UserRole;
using OrderPaymentSystem.Application.Extensions;
using OrderPaymentSystem.Domain.Errors;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.UserRole;

public class CreateUserRoleValidator : AbstractValidator<CreateUserRoleDto>
{
    public CreateUserRoleValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithError(DomainErrors.Validation.Required(nameof(CreateUserRoleDto.UserId)));

        RuleFor(x => x.RoleId)
            .NotEmpty()
            .WithError(DomainErrors.Validation.Required(nameof(CreateUserRoleDto.RoleId)));
    }
}
