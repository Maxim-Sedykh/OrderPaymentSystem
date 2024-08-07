﻿using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using OrderPaymentSystem.Domain.Constants;
using OrderPaymentSystem.Domain.Dto.Token;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Api.Controllers
{
    /// <summary>
    /// Контроллер для обновления токена пользователя
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
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
        /// 
        [Route(RouteConstants.RefreshToken)]
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
