namespace OrderPaymentSystem.Domain.Dto.UserRole;

public record UpdateUserRoleDto(
        long FromRoleId,
        long ToRoleId
    );
