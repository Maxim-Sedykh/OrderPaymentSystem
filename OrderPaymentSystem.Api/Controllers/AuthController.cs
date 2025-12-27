using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using OrderPaymentSystem.Application.Validations.FluentValidations.Auth;
using OrderPaymentSystem.Domain.Dto.Auth;
using OrderPaymentSystem.Domain.Dto.Token;
using OrderPaymentSystem.Domain.Interfaces.Services;

namespace OrderPaymentSystem.Api.Controllers;

/// <summary>
/// Контроллер для работы с авторизацией
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserTokenService _userTokenService;
    private readonly LoginUserValidator _loginUserValidator;
    private readonly RegisterUserValidation _registerUserValidator;

    /// <summary>
    /// Конструктор контроллера
    /// </summary>
    /// <param name="authService">Сервис аутентификации</param>
    /// <param name="loginUserValidator">Валидатор для модели аутентификации</param>
    /// <param name="registerUserValidator">Валидатор для модели регистрации</param>
    public AuthController(IAuthService authService, LoginUserValidator loginUserValidator,
        RegisterUserValidation registerUserValidator)
    {
        _authService = authService;
        _loginUserValidator = loginUserValidator;
        _registerUserValidator = registerUserValidator;
    }

    /// <summary>
    /// Регистрация пользователя
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    /// <returns></returns>
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterUserDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _registerUserValidator.ValidateAsync(dto, cancellationToken);

        if (!validationResult.IsValid)
        {
            return UnprocessableEntity(validationResult.Errors);
        }

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
        var validationResult = await _loginUserValidator.ValidateAsync(dto, cancellationToken);

        if (!validationResult.IsValid)
        {
            return UnprocessableEntity(validationResult.Errors);
        }

        var response = await _authService.LoginAsync(dto, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
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
