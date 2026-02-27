namespace OrderPaymentSystem.Application.DTOs.Auth;

/// <summary>
/// Модель данных для аутентификации пользователя
/// </summary>
/// <param name="Login">Логин</param>
/// <param name="Password">Введёный пароль</param>
public record LoginUserDto(string Login, string Password);
