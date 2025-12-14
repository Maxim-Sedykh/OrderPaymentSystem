using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderPaymentSystem.Application.Validations.FluentValidations.Role;
using OrderPaymentSystem.Application.Validations.FluentValidations.UserRole;
using OrderPaymentSystem.Domain.Dto.Role;
using OrderPaymentSystem.Domain.Dto.UserRole;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces.Services;
using System.Net.Mime;

namespace OrderPaymentSystem.Api.Controllers;

/// <summary>
/// Контроллер для работы с ролями пользователя
/// </summary>
[Authorize(Roles = "Admin")]
[Consumes(MediaTypeNames.Application.Json)]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/roles")]
[ApiController]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;
    private readonly CreateRoleValidation _createRoleValidation;
    private readonly UpdateRoleValidator _updateRoleValidator;

    /// <summary>
    /// Конструктор для работы с ролями
    /// </summary>
    /// <param name="roleService">Сервис для работы с ролями</param>
    /// <param name="createRoleValidation">Валидатор создания роли</param>
    /// <param name="updateRoleValidator">Валидатор роли</param>
    public RoleController(IRoleService roleService,
        CreateRoleValidation createRoleValidation,
        UpdateUserRoleValidation updateUserRoleValidation,
        UserRoleValidation userRoleValidation,
        UpdateRoleValidator updateRoleValidator)
    {
        _roleService = roleService;
        _createRoleValidation = createRoleValidation;
        _updateRoleValidator = updateRoleValidator;
    }

    /// <summary>
    /// Создание роли
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    /// <remarks>
    /// 
    ///     POST
    ///     {
    ///         "name": "User",
    ///     }
    ///     
    /// </remarks>
    /// <response code="200">Если роль создалась</response>
    /// <response code="400">Если роль не была создана</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RoleDto>> CreateRole(CreateRoleDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _createRoleValidation.ValidateAsync(dto, cancellationToken);

        if (!validationResult.IsValid)
        {
            return UnprocessableEntity(validationResult.Errors);
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
    /// <param name="id"></param>
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
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RoleDto>> UpdateRole(int id, UpdateRoleDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _updateRoleValidator.ValidateAsync(dto, cancellationToken);

        if (!validationResult.IsValid)
        {
            return UnprocessableEntity(validationResult.Errors);
        }

        var response = await _roleService.UpdateRoleAsync(id, dto, cancellationToken);

        if (response.IsSuccess)
            return Ok(response.Data);

        if (response.Error.Code == (int)ErrorCodes.RoleNotFound)
            return NotFound();

        return BadRequest(response.Error);
    }

    /// <summary>
    /// Удаление роли с указанием идентификатора
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    /// <remarks>
    /// 
    ///     DELETE
    ///     {
    ///         "id": 3
    ///     }
    ///     
    /// </remarks>
    /// <response code="200">Если роль удалилась</response>
    /// <response code="400">Если роль не была удалена</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RoleDto>> DeleteRole(long id, CancellationToken cancellationToken)
    {
        var response = await _roleService.DeleteRoleAsync(id, cancellationToken);
        if (response.IsSuccess)
        {
            return NoContent();
        }
        return BadRequest(response.Error);
    }

    /// <summary>
    /// Получение ролей пользователя
    /// </summary>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RoleDto[]>> GetAllRoles(CancellationToken cancellationToken)
    {
        var response = await _roleService.GetAllRolesAsync(cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
    }
}
