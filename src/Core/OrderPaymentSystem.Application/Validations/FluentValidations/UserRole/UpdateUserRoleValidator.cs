using FluentValidation;
using OrderPaymentSystem.Application.DTOs.UserRole;
using OrderPaymentSystem.Application.Extensions;
using OrderPaymentSystem.Domain.Errors;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.UserRole;

/// <summary>
/// Валидатор для <see cref="UpdateUserRoleDto"/>
/// </summary>
public class UpdateUserRoleValidator : AbstractValidator<UpdateUserRoleDto>
{
    /// <summary>
    /// Конструктор валидатора, создание правил для полей DTO <see cref="UpdateUserRoleDto"/>
    /// </summary>
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
