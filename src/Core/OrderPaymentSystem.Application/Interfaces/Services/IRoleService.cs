using OrderPaymentSystem.Application.DTOs.Role;
using OrderPaymentSystem.Shared.Result;

namespace OrderPaymentSystem.Application.Interfaces.Services;

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
    Task<CollectionResult<RoleDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавление роли
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns></returns>
    Task<DataResult<RoleDto>> CreateAsync(CreateRoleDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаление роли по идентификатору
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns></returns>
    Task<BaseResult> DeleteByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновление роли
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns></returns>
    Task<DataResult<RoleDto>> UpdateAsync(int id, UpdateRoleDto dto, CancellationToken cancellationToken = default);
}
