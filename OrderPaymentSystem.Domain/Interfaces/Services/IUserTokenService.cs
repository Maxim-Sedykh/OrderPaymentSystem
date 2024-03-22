﻿using OrderPaymentSystem.Domain.Dto;
using OrderPaymentSystem.Domain.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Domain.Interfaces.Services
{
    public interface IUserTokenService
    {
        /// <summary>
        /// Генерация Access-токена
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        string GenerateAccessToken(IEnumerable<Claim> claims);

        /// <summary>
        /// Генерация Refresh-токена
        /// </summary>
        /// <returns></returns>
        string GenerateRefreshToken();

        /// <summary>
        /// Получение ClaimsPrincipal из исчезающего токена
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken);

        /// <summary>
        /// Обновление токена пользователя
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<BaseResult<TokenDto>> RefreshToken(TokenDto dto);
    }
}
