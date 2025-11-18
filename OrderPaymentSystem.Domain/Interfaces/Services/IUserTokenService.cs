using OrderPaymentSystem.Domain.Dto.Token;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Result;
using System.Security.Claims;

namespace OrderPaymentSystem.Domain.Interfaces.Services;

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
    Task<DataResult<TokenDto>> RefreshTokenAsync(TokenDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение основных клаймов из пользователя
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    IReadOnlyCollection<Claim> GetClaimsFromUser(User user);
}
