namespace OrderPaymentSystem.Domain.Dto.Auth;

public record RegisterUserDto(string Login, string Password, string PasswordConfirm);
