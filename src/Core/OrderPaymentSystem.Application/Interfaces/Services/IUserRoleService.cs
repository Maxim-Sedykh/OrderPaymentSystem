using OrderPaymentSystem.Application.DTOs.UserRole;
using OrderPaymentSystem.Shared.Result;

namespace OrderPaymentSystem.Application.Interfaces.Services;

/// <summary>
/// Интерфейс сервиса для взаимодействия с ролями и пользователями
/// </summary>
public interface IUserRoleService
{
    /// <summary>
    /// Добавление роли для пользователя
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="roleName">Название роли</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Созданная роль пользователя</returns>
    Task<DataResult<UserRoleDto>> CreateAsync(Guid userId, string roleName, CancellationToken ct = default);

    /// <summary>
    /// Удаление роли у пользователя
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="roleId">Id роли</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Удалённая роль пользователя</returns>
    Task<DataResult<UserRoleDto>> DeleteAsync(Guid userId, int roleId, CancellationToken ct = default);

    /// <summary>
    /// Обновление роли у пользователя
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="dto">Модель данных для обновления роли пользователя</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Обновлённая роль пользователя</returns>
    Task<DataResult<UserRoleDto>> UpdateAsync(Guid userId, UpdateUserRoleDto dto, CancellationToken ct = default);

    /// <summary>
    /// Получение ролей пользователя
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Коллекция ролей пользователя</returns>
    Task<CollectionResult<string>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
}
