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
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Коллекция всех ролей</returns>
    Task<CollectionResult<RoleDto>> GetAllAsync(CancellationToken ct = default);

    /// <summary>
    /// Добавление роли
    /// </summary>
    /// <param name="dto">Модель создания роли</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Созданная роль</returns>
    Task<DataResult<RoleDto>> CreateAsync(CreateRoleDto dto, CancellationToken ct = default);

    /// <summary>
    /// Удаление роли по идентификатору
    /// </summary>
    /// <param name="id">Id роли</param>
    /// <param name="ct">Токен отмены операции</param>
    Task<BaseResult> DeleteByIdAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Обновление роли
    /// </summary>
    /// <param name="id">Id роли</param>
    /// <param name="dto">Модель для обновления роли</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Обновлённая роль</returns>
    Task<DataResult<RoleDto>> UpdateAsync(int id, UpdateRoleDto dto, CancellationToken ct = default);
}
