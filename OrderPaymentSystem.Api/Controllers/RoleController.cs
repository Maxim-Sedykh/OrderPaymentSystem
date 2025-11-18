using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderPaymentSystem.Application.Validations.FluentValidations.Role;
using OrderPaymentSystem.Application.Validations.FluentValidations.UserRole;
using OrderPaymentSystem.Domain.Constants;
using OrderPaymentSystem.Domain.Dto.Role;
using OrderPaymentSystem.Domain.Dto.UserRole;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Interfaces.Services;
using System.Net.Mime;

namespace OrderPaymentSystem.Api.Controllers;

/// <summary>
/// Контроллер для работы с ролями пользователя
/// </summary>
[Authorize(Roles = "Admin")]
[Consumes(MediaTypeNames.Application.Json)]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;
    private readonly CreateRoleValidation _createRoleValidation;
    private readonly DeleteUserRoleValidation _deleteUserRoleValidation;
    private readonly UpdateUserRoleValidation _updateUserRoleValidation;
    private readonly UserRoleValidation _userRoleValidation;
    private readonly RoleValidation _roleValidation;

    /// <summary>
    /// Конструктор для работы с ролями
    /// </summary>
    /// <param name="roleService">Сервис для работы с ролями</param>
    /// <param name="createRoleValidation">Валидатор создания роли</param>
    /// <param name="deleteUserRoleValidation">Валидатор удаления роли для пользователя</param>
    /// <param name="updateUserRoleValidation">Обновление роли для пользователя</param>
    /// <param name="userRoleValidation">Валидатор роли для пользователя</param>
    /// <param name="roleValidation">Валидатор роли</param>
    public RoleController(IRoleService roleService, CreateRoleValidation createRoleValidation,
        DeleteUserRoleValidation deleteUserRoleValidation, UpdateUserRoleValidation updateUserRoleValidation,
        UserRoleValidation userRoleValidation, RoleValidation roleValidation)
    {
        _roleService = roleService;
        _createRoleValidation = createRoleValidation;
        _deleteUserRoleValidation = deleteUserRoleValidation;
        _updateUserRoleValidation = updateUserRoleValidation;
        _userRoleValidation = userRoleValidation;
        _roleValidation = roleValidation;
    }

    /// <summary>
    /// Создание роли
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    /// <remarks>
    /// Request for create role:
    /// 
    ///     POST
    ///     {
    ///         "name": "User",
    ///     }
    ///     
    /// </remarks>
    /// <response code="200">Если роль создалась</response>
    /// <response code="400">Если роль не была создана</response>
    [HttpPost(RouteConstants.CreateRole)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Role>> CreateRole(CreateRoleDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _createRoleValidation.ValidateAsync(dto, cancellationToken);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var response = await _roleService.CreateRoleAsync(dto, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
    }

    /// <summary>
    /// Обновление роли
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    /// <remarks>
    /// Request for update role:
    /// 
    ///     PUT
    ///     {
    ///         "id": 1,
    ///         "name": "Admin",
    ///     }
    ///     
    /// </remarks>
    /// <response code="200">Если роль обновлена</response>
    /// <response code="400">Если роль не была обновлена</response>
    [HttpPut(RouteConstants.UpdateRole)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Role>> UpdateRole(RoleDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _roleValidation.ValidateAsync(dto, cancellationToken);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var response = await _roleService.UpdateRoleAsync(dto, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
    }

    /// <summary>
    /// Удаление роли с указанием идентификатора
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    /// <remarks>
    /// Request for delete role:
    /// 
    ///     DELETE
    ///     {
    ///         "id": 3
    ///     }
    ///     
    /// </remarks>
    /// <response code="200">Если роль удалилась</response>
    /// <response code="400">Если роль не была удалена</response>
    [HttpDelete(RouteConstants.DeleteRoleById)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Role>> DeleteRole(long id, CancellationToken cancellationToken)
    {
        var response = await _roleService.DeleteRoleAsync(id, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
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
    [HttpPost(RouteConstants.AddRoleForUser)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Role>> AddRoleForUser(UserRoleDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _userRoleValidation.ValidateAsync(dto, cancellationToken);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
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
    /// <param name="dto"></param>
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
    [HttpDelete(RouteConstants.DeleteRoleForUser)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Role>> DeleteRoleForUser(DeleteUserRoleDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _deleteUserRoleValidation.ValidateAsync(dto, cancellationToken);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var response = await _roleService.DeleteRoleForUserAsync(dto, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
    }

    /// <summary>
    /// Обновление роли пользователя
    /// </summary>
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
    [HttpPut(RouteConstants.UpdateRoleForUser)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Role>> UpdateRoleForUser(UpdateUserRoleDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _updateUserRoleValidation.ValidateAsync(dto, cancellationToken);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var response = await _roleService.UpdateRoleForUserAsync(dto, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
    }

    /// <summary>
    /// Получение ролей пользователя
    /// </summary>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    [HttpGet(RouteConstants.GetAllRoles)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RoleDto>> GetAllRoles(CancellationToken cancellationToken)
    {
        var response = await _roleService.GetAllRolesAsync(cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
    }
}
