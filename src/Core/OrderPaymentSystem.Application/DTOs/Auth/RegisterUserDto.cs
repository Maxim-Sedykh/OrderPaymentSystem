namespace OrderPaymentSystem.Application.DTOs.Auth;

public record RegisterUserDto(string Login, string Password, string PasswordConfirm);
