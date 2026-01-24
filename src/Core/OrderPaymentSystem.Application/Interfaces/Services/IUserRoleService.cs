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
    /// <param name="dto"></param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns></returns>
    Task<DataResult<UserRoleDto>> CreateAsync(CreateUserRoleDto dto, CancellationToken ct = default);

    /// <summary>
    /// Удаление роли у пользователя
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="roleId"></param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns></returns>
    Task<DataResult<UserRoleDto>> DeleteAsync(Guid userId, int roleId, CancellationToken ct = default);

    /// <summary>
    /// Обновление роли у пользователя
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="dto"></param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns></returns>
    Task<DataResult<UserRoleDto>> UpdateAsync(Guid userId, UpdateUserRoleDto dto, CancellationToken ct = default);

    /// <summary>
    /// Получение ролей пользователя
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns></returns>
    Task<CollectionResult<string>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
}
