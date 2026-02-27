namespace OrderPaymentSystem.Application.DTOs.Auth;

/// <summary>
/// Модель данных для регистрации пользователя
/// </summary>
/// <param name="Login">Логин</param>
/// <param name="Password">Пароль</param>
/// <param name="PasswordConfirm">Подтверждение пароля</param>
public record RegisterUserDto(string Login, string Password, string PasswordConfirm);
