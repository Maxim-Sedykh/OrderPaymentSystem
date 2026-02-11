using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Shared.Exceptions;

namespace OrderPaymentSystem.Domain.Entities;

/// <summary>
/// Сущность-связка роли и пользователя
/// </summary>
public class UserRole
{
    /// <summary>
    /// Id пользователя
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Id роли
    /// </summary>
    public int RoleId { get; private set; }

    private UserRole() { }

    /// <summary>
    /// Создать роль для пользователя
    /// </summary>
    /// <param name="userId">Id пользователя</param>
    /// <param name="roleId">Id роли</param>
    /// <returns>Результат создания</returns>
    public static UserRole Create(Guid userId, int roleId)
    {
        if (userId == default)
            throw new BusinessException(DomainErrors.Validation.Required(nameof(userId)));

        if (roleId == default || roleId < 0)
            throw new BusinessException(DomainErrors.Validation.Required(nameof(roleId)));

        return new UserRole { UserId = userId, RoleId = roleId };
    }
}
