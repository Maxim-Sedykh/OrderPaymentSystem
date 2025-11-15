namespace OrderPaymentSystem.Domain.Dto.Auth;

public record UserDto(string Login, string Password, DateTime CreatedAt);
