namespace OrderPaymentSystem.Application.DTOs.UserRole;

public record UpdateUserRoleDto(
        int FromRoleId,
        int ToRoleId
    );
