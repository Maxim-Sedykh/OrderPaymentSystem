using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderPaymentSystem.Application.Validations.FluentValidations.Role;
using OrderPaymentSystem.Application.Validations.FluentValidations.UserRole;
using OrderPaymentSystem.Domain.Dto.Role;
using OrderPaymentSystem.Domain.Dto.UserRole;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Result;
using System.Net.Mime;

namespace OrderPaymentSystem.Api.Controllers
{
    /// <summary>
    /// Контроллер для работы с ролями пользователя
    /// </summary>
    [Authorize(Roles = "Admin")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly CreateRoleValidation _createRoleValidation;
        private readonly DeleteUserRoleValidation _deleteUserRoleValidation;
        private readonly UpdateUserRoleValidation _updateUserRoleValidation;
        private readonly UserRoleValidation _userRoleValidation;
        private readonly RoleValidation _roleValidation;

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
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<Role>>> CreateRole([FromBody] CreateRoleDto dto)
        {
            var validationResult = await _createRoleValidation.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var response = await _roleService.CreateRoleAsync(dto);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        /// <summary>
        /// Обновление роли
        /// </summary>
        /// <param name="dto"></param>
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
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<Role>>> UpdateRole([FromBody] RoleDto dto)
        {
            var validationResult = await _roleValidation.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var response = await _roleService.UpdateRoleAsync(dto);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        /// <summary>
        /// Удаление роли с указанием идентификатора
        /// </summary>
        /// <param name="id"></param>
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
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<Role>>> DeleteRole(long id)
        {
            var response = await _roleService.DeleteRoleAsync(id);
            if (response.IsSuccess)
            { 
                return Ok(response);
            }
            return BadRequest(response);
        }

        /// <summary>
        /// Создание роли для пользователя
        /// </summary>
        /// <param name="dto"></param>
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
        [HttpPost("add-user-role")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<Role>>> AddRoleForUser([FromBody] UserRoleDto dto)
        {
            var validationResult = await _userRoleValidation.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var response = await _roleService.AddRoleForUserAsync(dto);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        /// <summary>
        /// Удаление роли у пользователя
        /// </summary>
        /// <param name="dto"></param>
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
        [HttpDelete("delete-user-role")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<Role>>> DeleteRoleForUser([FromBody] DeleteUserRoleDto dto)
        {
            var validationResult = await _deleteUserRoleValidation.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var response = await _roleService.DeleteRoleForUserAsync(dto);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        /// <summary>
        /// Обновление роли пользователя
        /// </summary>
        /// <param name="dto"></param>
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
        [HttpPut("update-user-role")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<Role>>> UpdateRoleForUser([FromBody] UpdateUserRoleDto dto)
        {
            var validationResult = await _updateUserRoleValidation.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var response = await _roleService.UpdateRoleForUserAsync(dto);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        /// <summary>
        /// Получение ролей пользоватекля
        /// </summary>
        [HttpGet("get-all-roles")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CollectionResult<RoleDto>>> GetAllRoles()
        {
            var response = await _roleService.GetAllRoles();
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
