using FluentValidation;
using OrderPaymentSystem.Application.DTOs.Role;
using OrderPaymentSystem.Application.Extensions;
using OrderPaymentSystem.Domain.Errors;
using static OrderPaymentSystem.Domain.Constants.ValidationConstants.Role;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.Role;

public class UpdateRoleValidator : AbstractValidator<UpdateRoleDto>
{
    public UpdateRoleValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithError(DomainErrors.Validation.Required(nameof(UpdateRoleDto.Name)))
            .MaximumLength(MaxNameLength)
            .WithError(DomainErrors.Validation.TooLong(nameof(UpdateRoleDto.Name), MaxNameLength));
    }
}
