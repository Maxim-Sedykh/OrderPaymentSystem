using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderPaymentSystem.Application.Validations.FluentValidations.UserRole;
using OrderPaymentSystem.Domain.Dto.UserRole;
using OrderPaymentSystem.Domain.Interfaces.Services;
using System.Net.Mime;

namespace OrderPaymentSystem.Api.Controllers;

/// <summary>
/// Контроллер для работы с ролями пользователя
/// </summary>
[Authorize(Roles = "Admin")]
[Consumes(MediaTypeNames.Application.Json)]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/roles/users")]
[ApiController]
public class UserRoleController : ControllerBase
{
    private readonly IRoleService _roleService;
    private readonly UpdateUserRoleValidation _updateUserRoleValidation;
    private readonly UserRoleValidation _userRoleValidation;

    /// <summary>
    /// Конструктор контроллера для работы с ролями пользователя
    /// </summary>
    /// <param name="roleService"></param>
    /// <param name="updateUserRoleValidation">Обновление роли для пользователя</param>
    /// <param name="userRoleValidation">Валидатор роли для пользователя</param>
    public UserRoleController(IRoleService roleService,
        UpdateUserRoleValidation updateUserRoleValidation,
        UserRoleValidation userRoleValidation)
    {
        _roleService = roleService;
        _updateUserRoleValidation = updateUserRoleValidation;
        _userRoleValidation = userRoleValidation;
    }

    /// <summary>
    /// Создание роли для пользователя
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    /// <remarks>
    /// Request for add role for user
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
    [HttpPost("/users")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserRoleDto>> AddRoleForUser(UserRoleDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _userRoleValidation.ValidateAsync(dto, cancellationToken);

        if (!validationResult.IsValid)
        {
            return UnprocessableEntity(validationResult.Errors);
        }

        var response = await _roleService.AddRoleForUserAsync(dto, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
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
    [HttpDelete("{roleId}/users/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserRoleDto>> DeleteRoleForUser(int roleId, Guid userId, CancellationToken cancellationToken)
    {
        var response = await _roleService.DeleteRoleForUserAsync(userId, roleId, cancellationToken);
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
    /// <remarks>
    /// Request for update role for user
    /// 
    ///     POST
    ///     {
    ///         "login": "User first",
    ///         "fromRoleId": 7,
    ///         "ToRoleId": 1,
    ///     }
    ///     
    /// </remarks>
    /// <response code="200">Если роль для пользователя обновилась</response>
    /// <response code="400">Если роль для пользователя не была удалена</response>
    [HttpPut("/users/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserRoleDto>> UpdateRoleForUser(Guid userId, UpdateUserRoleDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _updateUserRoleValidation.ValidateAsync(dto, cancellationToken);

        if (!validationResult.IsValid)
        {
            return UnprocessableEntity(validationResult.Errors);
        }

        var response = await _roleService.UpdateRoleForUserAsync(userId, dto, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
    }
}
