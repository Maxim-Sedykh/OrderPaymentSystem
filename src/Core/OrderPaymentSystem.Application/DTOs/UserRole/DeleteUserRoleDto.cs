namespace OrderPaymentSystem.Application.DTOs.UserRole;

/// <summary>
/// Удалить роль для пользователя
/// </summary>
/// <param name="Login">Логин пользователя</param>
/// <param name="RoleId">Id роли</param>
public record DeleteUserRoleDto(
        string Login,
        long RoleId
    );
