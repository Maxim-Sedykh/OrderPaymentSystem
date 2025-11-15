using OrderPaymentSystem.Domain.Dto.Token;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Result;
using System.Security.Claims;

namespace OrderPaymentSystem.Domain.Interfaces.Services;

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
    /// Получение ClaimsPrincipal из исчезающего токена
    /// </summary>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken);

    /// <summary>
    /// Обновление токена пользователя
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<BaseResult<TokenDto>> RefreshToken(TokenDto dto);

    /// <summary>
    /// Получение основных клаймов из пользователя
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    List<Claim> GetClaimsFromUser(User user);
}
