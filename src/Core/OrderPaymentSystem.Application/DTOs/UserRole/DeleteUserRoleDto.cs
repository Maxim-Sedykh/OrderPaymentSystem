namespace OrderPaymentSystem.Application.DTOs.UserRole;

public record DeleteUserRoleDto(
        string Login,
        long RoleId
    );
