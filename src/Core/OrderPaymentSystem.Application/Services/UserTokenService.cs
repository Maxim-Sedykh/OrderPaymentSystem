using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OrderPaymentSystem.Application.Constants;
using OrderPaymentSystem.Application.DTOs.Token;
using OrderPaymentSystem.Application.Interfaces.Cache;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Application.Interfaces.Services;
using OrderPaymentSystem.Application.Settings;
using OrderPaymentSystem.Application.Specifications;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Domain.Resources;
using OrderPaymentSystem.Shared.Result;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace OrderPaymentSystem.Application.Services;

/// <summary>
/// Сервис для работы с JWT-токенами
/// </summary>
public class UserTokenService : IUserTokenService
{
    private readonly string _jwtKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;

    public UserTokenService(
        IOptions<JwtSettings> options,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider = null)
    {
        _jwtKey = options.Value.JwtKey;
        _issuer = options.Value.Issuer;
        _audience = options.Value.Audience;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    /// <inheritdoc/>
    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: _timeProvider.GetUtcNow().UtcDateTime.AddMinutes(10),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }

    /// <inheritdoc/>
    public string GenerateRefreshToken()
    {
        var randomNumbers = new byte[32];
        using var randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(randomNumbers);

        return Convert.ToBase64String(randomNumbers);
    }

    /// <inheritdoc/>
    public IReadOnlyCollection<Claim> GetClaimsFromUser(User user)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));

        if (user.Roles == null || !user.Roles.Any())
        {
            throw new InvalidOperationException(ErrorMessage.UserRolesNotFound);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Login),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };

        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role.Name)));

        return claims.AsReadOnly();
    }

    /// <inheritdoc/>
    public async Task<DataResult<TokenDto>> RefreshAsync(
        TokenDto dto,
        CancellationToken ct = default)
    {
        var userResult = await GetValidUserForRefreshAsync(dto, ct);
        if (!userResult.IsSuccess)
        {
            return DataResult<TokenDto>.Failure(userResult.Error);
        }

        var user = userResult.Data;
        var newClaims = GetClaimsFromUser(user);
        var newAccessToken = GenerateAccessToken(newClaims);
        var newRefreshToken = GenerateRefreshToken();

        user.UserToken.UpdateRefreshTokenData(
            newRefreshToken,
            _timeProvider.GetUtcNow().UtcDateTime.AddDays(7)
        );

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(ct);

        var tokenDto = new TokenDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
        };

        return DataResult<TokenDto>.Success(tokenDto);
    }

    /// <summary>
    /// Получает и валидирует пользователя для обновления токена
    /// </summary>
    /// <param name="dto">DTO с токенами</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Валидный пользователь или ошибка</returns>
    private async Task<DataResult<User>> GetValidUserForRefreshAsync(
        TokenDto dto,
        CancellationToken ct)
    {
        ClaimsPrincipal claimsPrincipal;
        try
        {
            claimsPrincipal = GetPrincipalFromExpiredToken(dto.AccessToken);
        }
        catch (SecurityTokenException)
        {
            return DataResult<User>.Failure(DomainErrors.General.InvalidToken());
        }

        var login = claimsPrincipal.Identity?.Name;
        if (string.IsNullOrEmpty(login))
        {
            return DataResult<User>.Failure(DomainErrors.General.InvalidToken());
        }

        var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(UserSpecs.ByLogin(login).ForAuth(), ct);

        if (user?.UserToken == null)
        {
            return DataResult<User>.Failure(DomainErrors.User.NotFoundByLogin(login));
        }

        if (user.UserToken.RefreshToken != dto.RefreshToken)
        {
            return DataResult<User>.Failure(DomainErrors.General.InvalidClientRequest());
        }

        if (user.UserToken.RefreshTokenExpireTime <= _timeProvider.GetUtcNow().UtcDateTime)
        {
            return DataResult<User>.Failure(DomainErrors.Token.RefreshExpired());
        }

        return DataResult<User>.Success(user);
    }

    /// <summary>
    /// Получение ClaimsPrincipal из истекшего токена
    /// </summary>
    /// <param name="accessToken">Access token</param>
    /// <returns>ClaimsPrincipal</returns>
    /// <exception cref="SecurityTokenException">Когда токен невалидный</exception>
    private ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey)),
            ValidateLifetime = false,
            ValidAudience = _audience,
            ValidIssuer = _issuer,
            ClockSkew = TimeSpan.Zero
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var claimsPrincipal = tokenHandler.ValidateToken(
            accessToken,
            tokenValidationParameters,
            out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
        {
            throw new SecurityTokenException(ErrorMessage.InvalidToken);
        }

        return claimsPrincipal;
    }
}