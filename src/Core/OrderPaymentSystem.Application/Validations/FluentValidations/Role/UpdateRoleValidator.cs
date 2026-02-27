using FluentValidation;
using OrderPaymentSystem.Application.DTOs.Role;
using OrderPaymentSystem.Application.Extensions;
using OrderPaymentSystem.Domain.Errors;
using static OrderPaymentSystem.Domain.Constants.ValidationConstants.Role;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.Role;

/// <summary>
/// Валидатор для <see cref="UpdateRoleDto"/>
/// </summary>
public class UpdateRoleValidator : AbstractValidator<UpdateRoleDto>
{
    /// <summary>
    /// Конструктор валидатора, создание правил для полей DTO <see cref="UpdateRoleDto"/>
    /// </summary>
    public UpdateRoleValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithError(DomainErrors.Validation.Required(nameof(UpdateRoleDto.Name)))
            .MaximumLength(MaxNameLength)
            .WithError(DomainErrors.Validation.TooLong(nameof(UpdateRoleDto.Name), MaxNameLength));
    }
}
