namespace OrderPaymentSystem.Application.DTOs.Auth;

public record UserDto(string Login, string Password, DateTime CreatedAt);
