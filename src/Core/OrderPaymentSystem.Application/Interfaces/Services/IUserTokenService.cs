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
    /// <param name="claims"></param>
    /// <returns></returns>
    string GenerateAccessToken(IEnumerable<Claim> claims);

    /// <summary>
    /// Генерация Refresh-токена
    /// </summary>
    /// <returns></returns>
    string GenerateRefreshToken();

    /// <summary>
    /// Обновление токена пользователя
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<DataResult<TokenDto>> RefreshAsync(TokenDto dto, CancellationToken ct = default);

    /// <summary>
    /// Получение основных клаймов из пользователя
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    CollectionResult<Claim> GetClaimsFromUser(User user);
}
