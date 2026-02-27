using FluentValidation;
using OrderPaymentSystem.Application.DTOs.Role;
using OrderPaymentSystem.Application.Extensions;
using OrderPaymentSystem.Domain.Errors;
using static OrderPaymentSystem.Domain.Constants.ValidationConstants.Role;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.Role;

/// <summary>
/// Валидатор для <see cref="CreateRoleDto"/>
/// </summary>
public class CreateRoleValidator : AbstractValidator<CreateRoleDto>
{
    /// <summary>
    /// Конструктор валидатора, создание правил для полей DTO <see cref="CreateRoleDto"/>
    /// </summary>
    public CreateRoleValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithError(DomainErrors.Validation.Required(nameof(CreateRoleDto.Name)))
            .MaximumLength(MaxNameLength)
            .WithError(DomainErrors.Validation.TooLong(nameof(CreateRoleDto.Name), MaxNameLength));
    }
}
