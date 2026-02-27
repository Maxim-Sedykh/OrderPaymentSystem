namespace OrderPaymentSystem.Application.DTOs.UserRole;

/// <summary>
/// Модель данных для представления роли пользователя
/// </summary>
/// <param name="Login">Логин пользователя</param>
/// <param name="RoleName">Название роли</param>
public record UserRoleDto(
        string Login,
        string RoleName
    );
