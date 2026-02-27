namespace OrderPaymentSystem.Application.DTOs.UserRole;

/// <summary>
/// Модель для обновления роли для пользователя
/// </summary>
/// <param name="FromRoleId">Id старой роли</param>
/// <param name="ToRoleId">Id новой роли</param>
public record UpdateUserRoleDto(
        int FromRoleId,
        int ToRoleId
    );
