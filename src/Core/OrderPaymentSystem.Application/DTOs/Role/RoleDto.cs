namespace OrderPaymentSystem.Application.DTOs.Role;

/// <summary>
/// Модель для представления роли
/// </summary>
/// <param name="Id">Id роли</param>
/// <param name="Name">Название роли</param>
public sealed record RoleDto(long Id, string Name);
