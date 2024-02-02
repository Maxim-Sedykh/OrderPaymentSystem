using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderPaymentSystem.Application.Services;
using OrderPaymentSystem.Domain.Dto;
using OrderPaymentSystem.Domain.Dto.Auth;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserTokenController : Controller
    {
        private readonly IUserTokenService _userTokenService;

        public UserTokenController(IUserTokenService userTokenService)
        {
            _userTokenService = userTokenService;   
        }

        /// <summary>
        /// Обновление токена пользователя
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<BaseResult<TokenDto>>> RefreshToken([FromBody] TokenDto dto)
        {
            var response = await _userTokenService.RefreshToken(dto);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
