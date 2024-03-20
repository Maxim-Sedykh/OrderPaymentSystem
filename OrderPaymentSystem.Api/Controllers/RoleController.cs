using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderPaymentSystem.Application.Services;
using OrderPaymentSystem.Domain.Dto.Product;
using OrderPaymentSystem.Domain.Dto.Role;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Result;
using System.Net.Mime;

namespace OrderPaymentSystem.Api.Controllers
{
    [Consumes(MediaTypeNames.Application.Json)]
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Создание роли
        /// </summary>
        /// <param name="dto"></param>
        /// <remarks>
        /// Request for create role
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
        /// Sample request:
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
        /// Sample request:
        /// 
        ///     DELETE
        ///     {
        ///         "id": 1
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
        /// <response code="200">Если роль создалась</response>
        /// <response code="400">Если роль не была создана</response>
        [HttpPost("addRole")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<Role>>> AddRoleForUser([FromBody] UserRoleDto dto)
        {
            var response = await _roleService.AddRoleForUserAsync(dto);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
