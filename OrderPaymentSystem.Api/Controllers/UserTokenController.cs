using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using OrderPaymentSystem.Domain.Constants;
using OrderPaymentSystem.Domain.Dto.Token;
using OrderPaymentSystem.Domain.Interfaces.Services;

namespace OrderPaymentSystem.Api.Controllers;

/// <summary>
/// Контроллер для обновления токена пользователя
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class UserTokenController : Controller
{
    private readonly IUserTokenService _userTokenService;

    /// <summary>
    /// Конструктор для работы с токеном-JWT
    /// </summary>
    /// <param name="userTokenService"></param>
    public UserTokenController(IUserTokenService userTokenService)
    {
        _userTokenService = userTokenService;
    }

    /// <summary>
    /// Обновление токена пользователя
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    /// <returns></returns>
    [Route(RouteConstants.RefreshToken)]
    [HttpPost]
    public async Task<ActionResult<TokenDto>> RefreshToken(TokenDto dto, CancellationToken cancellationToken)
    {
        var response = await _userTokenService.RefreshTokenAsync(dto, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
    }
}
