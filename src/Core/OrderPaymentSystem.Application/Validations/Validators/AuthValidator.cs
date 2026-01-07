using OrderPaymentSystem.Application.Interfaces.Auth;
using OrderPaymentSystem.Application.Interfaces.Validators;
using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces.Auth;
using OrderPaymentSystem.Shared.Result;

namespace OrderPaymentSystem.Application.Validations.Validators;

/// <summary>
/// Валидатор для процесса авторизации пользователя
/// </summary>
/// <param name="passwordHasher">Сервис для хэширования пароля</param>
public class AuthValidator(IPasswordHasher passwordHasher) : IAuthValidator
{
    /// <inheritdoc/>
    public BaseResult ValidateLogin(User user, string enteredPassword)
    {
        if (user == null)
        {
            return BaseResult.Failure((int)ErrorCodes.UserNotFound, ErrorMessage.UserNotFound);
        }

        bool verified = passwordHasher.Verify(enteredPassword, passwordHash: user.Password);

        if (!verified)
        {
            return BaseResult.Failure((int)ErrorCodes.PasswordIsWrong, ErrorMessage.PasswordIsWrong);
        }

        return BaseResult.Success();
    }
}
