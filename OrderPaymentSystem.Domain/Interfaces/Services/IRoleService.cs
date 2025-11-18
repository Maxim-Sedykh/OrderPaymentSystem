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
    /// <returns></returns>
    Task<CollectionResult<RoleDto>> GetAllRolesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавление роли
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<DataResult<RoleDto>> CreateRoleAsync(CreateRoleDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаление роли по идентификатору
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<DataResult<RoleDto>> DeleteRoleAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновление роли
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<DataResult<RoleDto>> UpdateRoleAsync(RoleDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавление роли для пользователя
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<DataResult<UserRoleDto>> AddRoleForUserAsync(UserRoleDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаление роли у пользователя
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<DataResult<UserRoleDto>> DeleteRoleForUserAsync(DeleteUserRoleDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновление роли у пользователя
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<DataResult<UserRoleDto>> UpdateRoleForUserAsync(UpdateUserRoleDto dto, CancellationToken cancellationToken = default);
}
