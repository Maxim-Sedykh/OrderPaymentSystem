using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderPaymentSystem.Application.DTOs.UserRole;
using OrderPaymentSystem.Application.Interfaces.Services;
using OrderPaymentSystem.Application.Validations.FluentValidations.UserRole;
using OrderPaymentSystem.Domain.Enum;
using System.Net.Mime;

namespace OrderPaymentSystem.Api.Controllers;

/// <summary>
/// Контроллер для работы с ролями пользователя
/// </summary>
[Authorize(Roles = "Admin")]
[Consumes(MediaTypeNames.Application.Json)]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
[ApiController]
public class UserRoleController : ControllerBase
{
    private readonly IUserRoleService _userRoleService;
    private readonly UpdateUserRoleValidation _updateUserRoleValidation;
    private readonly CreateUserRoleValidation _createUserRoleValidation;

    /// <summary>
    /// Конструктор контроллера для работы с ролями пользователя
    /// </summary>
    /// <param name="userRoleService">Сервис для работы с ролями и пользователями</param>
    /// <param name="updateUserRoleValidation">Валидатор для обновления роли для пользователя</param>
    /// <param name="createUserRoleValidation">Валидатор для добавления роли к пользователю</param>
    public UserRoleController(IUserRoleService userRoleService,
        UpdateUserRoleValidation updateUserRoleValidation,
        CreateUserRoleValidation createUserRoleValidation)
    {
        _userRoleService = userRoleService;
        _updateUserRoleValidation = updateUserRoleValidation;
        _createUserRoleValidation = createUserRoleValidation;
    }

    /// <summary>
    /// Создание роли для пользователя
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    /// <remarks>
    /// 
    ///     POST
    ///     {
    ///         "login": "User first",
    ///         "roleName": "Admin",
    ///     }
    ///     
    /// </remarks>
    /// <response code="200">Если роль для пользователя создалась</response>
    /// <response code="400">Если роль для пользователя не была создана</response>
    [HttpPost("{userId}/roles")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserRoleDto>> AddRoleForUser(CreateUserRoleDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _createUserRoleValidation.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return UnprocessableEntity(validationResult.Errors);
        }

        var response = await _userRoleService.CreateAsync(dto, cancellationToken);
        if (response.IsSuccess)
        {
            return CreatedAtAction(nameof(AddRoleForUser), response.Data);
        }
        return BadRequest(response.Error);
    }

    /// <summary>
    /// Удаление роли у пользователя
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="userId"></param>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    /// <remarks>
    /// Request for delete role for user
    /// 
    ///     POST
    ///     {
    ///         "login": "User first",
    ///         "roleId": 1,
    ///     }
    ///     
    /// </remarks>
    /// <response code="200">Если роль для пользователя удалилась</response>
    /// <response code="400">Если роль для пользователя не была удалена</response>
    [HttpDelete("{userId}/roles/{roleId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserRoleDto>> DeleteRoleForUser(Guid userId, int roleId, CancellationToken cancellationToken)
    {
        var response = await _userRoleService.DeleteAsync(userId, roleId, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
    }

    /// <summary>
    /// Обновление роли пользователя
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="dto"></param>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    /// <response code="200">Если роль для пользователя обновилась</response>
    /// <response code="400">Если роль для пользователя не была удалена</response>
    [HttpPut("{userId}/roles")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserRoleDto>> UpdateRoleForUser(Guid userId, UpdateUserRoleDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _updateUserRoleValidation.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return UnprocessableEntity(validationResult.Errors);
        }

        var response = await _userRoleService.UpdateAsync(userId, dto, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
    }

    /// <summary>
    /// Получить роли пользователя
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    [HttpGet("{userId}/roles")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserRoleDto>> GetUserRoles(Guid userId, CancellationToken cancellationToken)
    {
        var response = await _userRoleService.GetByUserIdAsync(userId, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        if (response.Error.Code == (int)ErrorCodes.UserNotFound)
        {
            return NotFound(ErrorCodes.UserNotFound.ToString());
        }
        return BadRequest(response.Error);
    }
}
