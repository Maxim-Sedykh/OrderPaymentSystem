namespace OrderPaymentSystem.Application.DTOs.Role;

/// <summary>
/// Модель для обновления данных роли
/// </summary>
/// <param name="Name">Новое название роли</param>
public sealed record UpdateRoleDto(string Name);
