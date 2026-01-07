using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Shared.Result;

namespace OrderPaymentSystem.Application.Interfaces.Validators;

/// <summary>
/// Валидатор для взаимодействия с ролями
/// </summary>
public interface IRoleValidator
{
    /// <summary>
    /// Валидировать роли для пользователей
    /// </summary>
    /// <param name="user">Пользователь</param>
    /// <param name="roles">Роли</param>
    /// <returns>Результат валидации</returns>
    BaseResult ValidateRoleForUser(User user, params Role[] roles);
}
