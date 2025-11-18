using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using OrderPaymentSystem.Application.Validations.FluentValidations.Auth;
using OrderPaymentSystem.Domain.Constants;
using OrderPaymentSystem.Domain.Dto.Auth;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Api.Controllers;

/// <summary>
/// Контроллер для работы с авторизацией
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly LoginUserValidator _loginUserValidator;
    private readonly RegisterUserValidation _registerUserValidator;

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
    [HttpPost(RouteConstants.Register)]
    public async Task<ActionResult<BaseResult>> Register(RegisterUserDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _registerUserValidator.ValidateAsync(dto, cancellationToken);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var response = await _authService.RegisterAsync(dto, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    /// <summary>
    /// Логин пользователя
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    /// <returns></returns>
    [HttpPost(RouteConstants.Login)]
    public async Task<ActionResult<BaseResult>> Login(LoginUserDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _loginUserValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var response = await _authService.LoginAsync(dto, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }
}
