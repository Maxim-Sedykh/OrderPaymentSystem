using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderPaymentSystem.Application.DTOs.Auth;
using OrderPaymentSystem.Application.DTOs.Token;
using OrderPaymentSystem.Application.Interfaces.Services;

namespace OrderPaymentSystem.Api.Controllers;

/// <summary>
/// Контроллер для работы с авторизацией
/// </summary>
[AllowAnonymous]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserTokenService _userTokenService;

    /// <summary>
    /// Конструктор контроллера
    /// </summary>
    /// <param name="authService">Сервис аутентификации</param>
    /// <param name="userTokenService">Сервис работы с токенами</param>
    public AuthController(IAuthService authService, IUserTokenService userTokenService)
    {
        _authService = authService;
        _userTokenService = userTokenService;
    }

    /// <summary>
    /// Регистрация пользователя
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    /// <returns></returns>
    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterUserDto dto, CancellationToken cancellationToken)
    {
        var response = await _authService.RegisterAsync(dto, cancellationToken);
        if (response.IsSuccess)
        {
            return Created();
        }
        return BadRequest(response);
    }

    /// <summary>
    /// Логин пользователя
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<ActionResult<TokenDto>> Login(LoginUserDto dto, CancellationToken cancellationToken)
    {
        var response = await _authService.LoginAsync(dto, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return Unauthorized(response.Error);
    }

    /// <summary>
    /// Обновление токена пользователя
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    /// <returns></returns>
    [HttpPost("refresh")]
    public async Task<ActionResult<TokenDto>> RefreshToken(TokenDto dto, CancellationToken cancellationToken)
    {
        var response = await _userTokenService.RefreshAsync(dto, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
    }
}
