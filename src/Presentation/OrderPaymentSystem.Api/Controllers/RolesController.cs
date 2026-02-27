using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderPaymentSystem.Application.DTOs.Role;
using OrderPaymentSystem.Application.Interfaces.Services;
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
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    /// <summary>
    /// Конструктор для работы с ролями
    /// </summary>
    /// <param name="roleService">Сервис для работы с ролями</param>
    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    /// <summary>
    /// Создание роли
    /// </summary>
    /// <param name="dto">Модель данных для создания</param>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    /// <remarks>
    /// 
    ///     POST
    ///     {
    ///         "name": "User",
    ///     }
    ///     
    /// </remarks>
    /// <response code="201">Если роль создалась</response>
    /// <response code="400">Если роль не была создана</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RoleDto>> CreateRole(CreateRoleDto dto, CancellationToken cancellationToken)
    {
        var response = await _roleService.CreateAsync(dto, cancellationToken);
        if (response.IsSuccess)
        {
            return CreatedAtAction(nameof(GetAllRoles), response.Data);
        }
        return BadRequest(response.Error);
    }

    /// <summary>
    /// Обновление роли
    /// </summary>
    /// <param name="id">Id роли для обновления</param>
    /// <param name="dto">Модель с данными для обновления</param>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    /// <remarks>
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
        var response = await _roleService.UpdateAsync(id, dto, cancellationToken);

        if (response.IsSuccess)
            return Ok(response.Data);

        return BadRequest(response.Error);
    }

    /// <summary>
    /// Удаление роли с указанием идентификатора
    /// </summary>
    /// <param name="id">Id роли</param>
    /// <param name="cancellationToken">Токен отмены запроса</param>
    /// <remarks>
    /// 
    ///     DELETE
    ///     {
    ///         "id": 3
    ///     }
    ///     
    /// </remarks>
    /// <response code="204">Если роль удалилась</response>
    /// <response code="400">Если роль не была удалена</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeleteRole(int id, CancellationToken cancellationToken)
    {
        var response = await _roleService.DeleteByIdAsync(id, cancellationToken);
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
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetAllRoles(CancellationToken cancellationToken)
    {
        var response = await _roleService.GetAllAsync(cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Error);
    }
}
