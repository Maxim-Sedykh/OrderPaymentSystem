using OrderPaymentSystem.Application.DTOs.Token;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Shared.Result;
using System.Security.Claims;

namespace OrderPaymentSystem.Application.Interfaces.Services;

/// <summary>
/// Интерфейс сервиса для работы с JWT-токенами
/// </summary>
public interface IUserTokenService
{
    /// <summary>
    /// Генерация Access-токена
    /// </summary>
    /// <param name="claims">Клэймы пользователя</param>
    /// <returns>Access-токен</returns>
    string GenerateAccessToken(IEnumerable<Claim> claims);

    /// <summary>
    /// Генерация Refresh-токена
    /// </summary>
    /// <returns>Refresh-токен</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// Обновление токена пользователя
    /// </summary>
    /// <param name="dto">Модель токена</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Обновлённый токен</returns>
    Task<DataResult<TokenDto>> RefreshAsync(TokenDto dto, CancellationToken ct = default);

    /// <summary>
    /// Получение основных клаймов из пользователя
    /// </summary>
    /// <param name="user">Пользователь</param>
    /// <returns>Клеймы пользователя</returns>
    CollectionResult<Claim> GetClaimsFromUser(User user);
}
