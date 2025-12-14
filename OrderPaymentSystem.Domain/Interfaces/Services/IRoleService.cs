using OrderPaymentSystem.Domain.Dto.Role;
using OrderPaymentSystem.Domain.Dto.UserRole;
using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Domain.Interfaces.Services;

/// <summary>
/// Сервис, предназначенный для управления ролей
/// </summary>
public interface IRoleService
{
    /// <summary>
    /// Получение всех ролей
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns></returns>
    Task<CollectionResult<RoleDto>> GetAllRolesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавление роли
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns></returns>
    Task<DataResult<RoleDto>> CreateRoleAsync(CreateRoleDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаление роли по идентификатору
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns></returns>
    Task<BaseResult> DeleteRoleAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновление роли
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns></returns>
    Task<DataResult<RoleDto>> UpdateRoleAsync(int id, UpdateRoleDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавление роли для пользователя
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns></returns>
    Task<DataResult<UserRoleDto>> AddRoleForUserAsync(UserRoleDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаление роли у пользователя
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="roleId"></param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns></returns>
    Task<DataResult<UserRoleDto>> DeleteRoleForUserAsync(Guid userId, int roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновление роли у пользователя
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="dto"></param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns></returns>
    Task<DataResult<UserRoleDto>> UpdateRoleForUserAsync(Guid userId, UpdateUserRoleDto dto, CancellationToken cancellationToken = default);
}
