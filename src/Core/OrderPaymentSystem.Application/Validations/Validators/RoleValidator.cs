using OrderPaymentSystem.Application.Interfaces.Validators;
using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Shared.Result;

namespace OrderPaymentSystem.Application.Validations.Validators;

/// <summary>
/// Валидатор для взаимодействия с ролями.
/// </summary>
public class RoleValidator : IRoleValidator
{
    /// <inheritdoc/>
    public BaseResult ValidateRoleForUser(User user, params Role[] roles)
    {
        if (user == null)
        {
            return BaseResult.Failure((int)ErrorCodes.UserNotFound, ErrorMessage.UserNotFound);
        }

        foreach (Role role in roles)
        {
            if (role == null)
            {
                return BaseResult.Failure((int)ErrorCodes.RoleNotFound, ErrorMessage.RoleNotFound);
            }
        }

        return BaseResult.Success();
    }
}
