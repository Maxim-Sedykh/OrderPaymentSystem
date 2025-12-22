using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Domain.Entities;

/// <summary>
/// Сущность-связка роли и пользователя
/// </summary>
public class UserRole
{
    /// <summary>
    /// Id пользователя
    /// </summary>
    public Guid UserId { get; protected set; }

    /// <summary>
    /// Id роли
    /// </summary>
    public long RoleId { get; protected set; }

    protected UserRole() { }

    /// <summary>
    /// Создать роль для пользователя
    /// </summary>
    /// <param name="userId">Id пользователя</param>
    /// <param name="roleId">Id роли</param>
    /// <returns>Результат создания</returns>
    public static DataResult<UserRole> Create(Guid userId, long roleId)
    {
        return DataResult<UserRole>.Success(new UserRole { UserId = userId, RoleId = roleId });
    }
}
