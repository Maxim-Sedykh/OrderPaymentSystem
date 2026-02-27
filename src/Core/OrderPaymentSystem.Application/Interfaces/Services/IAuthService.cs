using OrderPaymentSystem.Application.DTOs.Auth;
using OrderPaymentSystem.Application.DTOs.Token;
using OrderPaymentSystem.Shared.Result;

namespace OrderPaymentSystem.Application.Interfaces.Services;

/// <summary>
/// Сервис отвечающий за работу с авторизацией и аутентификацией
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Регистрация пользователя
    /// </summary>
    /// <param name="dto">Модель для регистрации</param>
    /// <param name="ct">Токен отмены операции</param>
    Task<BaseResult> RegisterAsync(RegisterUserDto dto, CancellationToken ct = default);

    /// <summary>
    /// Авторизация пользователя
    /// </summary>
    /// <param name="dto">Модель для авторизации</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Модель для передачи токенов</returns>
    Task<DataResult<TokenDto>> LoginAsync(LoginUserDto dto, CancellationToken ct = default);
}
