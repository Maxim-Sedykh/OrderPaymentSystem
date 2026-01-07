namespace OrderPaymentSystem.Application.DTOs.UserRole;

public record UpdateUserRoleDto(
        long FromRoleId,
        long ToRoleId
    );
