using FluentValidation;
using OrderPaymentSystem.Application.DTOs.UserRole;
using OrderPaymentSystem.Application.Extensions;
using OrderPaymentSystem.Domain.Errors;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.UserRole;

public class UpdateUserRoleValidator : AbstractValidator<UpdateUserRoleDto>
{
    public UpdateUserRoleValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.FromRoleId)
            .NotEmpty()
            .WithError(DomainErrors.Validation.Required(nameof(UpdateUserRoleDto.FromRoleId)));

        RuleFor(x => x.ToRoleId)
            .NotEmpty()
            .WithError(DomainErrors.Validation.Required(nameof(UpdateUserRoleDto.ToRoleId)))
            .NotEqual(x => x.FromRoleId)
            .WithError(DomainErrors.Role.SameRoleSelected());
    }
}
