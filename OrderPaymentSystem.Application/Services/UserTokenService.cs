using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.Domain.Dto.Token;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Result;
using OrderPaymentSystem.Domain.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace OrderPaymentSystem.Application.Services;

/// <summary>
/// Cервис для работы с JWT-токенами
/// </summary>
/// <param name="options"></param>
/// <param name="userRepository"></param>
public class UserTokenService(IOptions<JwtSettings> options, IBaseRepository<User> userRepository) : IUserTokenService
{
    private readonly string _jwtKey = options.Value.JwtKey;
    private readonly string _issuer = options.Value.Issuer;
    private readonly string _audience = options.Value.Audience;

    /// <inheritdoc/>
    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var securityToken = new JwtSecurityToken(_issuer, _audience, claims, null, DateTime.UtcNow.AddMinutes(10), credentials);
        var token = new JwtSecurityTokenHandler().WriteToken(securityToken);

        return token;
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
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user), ErrorMessage.UserNotFound);
        }

        if (user.Roles == null)
        {
            throw new ArgumentNullException(nameof(user.Roles), ErrorMessage.UserRolesNotFound);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Login),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };

        claims.AddRange(user.Roles.Select(x => new Claim(ClaimTypes.Role, x.Name)));

        return claims;
    }

    /// <inheritdoc/>
    public async Task<DataResult<TokenDto>> RefreshTokenAsync(TokenDto dto, CancellationToken cancellationToken = default)
    {
        string accessToken = dto.AccessToken;
        string refreshToken = dto.RefreshToken;

        var claimsPrincipal = GetPrincipalFromExpiredToken(accessToken);
        var userName = claimsPrincipal.Identity?.Name;

        var user = await userRepository.GetQueryable()
            .Include(x => x.UserToken)
            .FirstOrDefaultAsync(x => x.Login == userName, cancellationToken);

        if (user == null || user.UserToken.RefreshToken != refreshToken ||
            user.UserToken.RefreshTokenExpireTime <= DateTime.UtcNow)
        {
            return DataResult<TokenDto>.Failure((int)ErrorCodes.InvalidClientRequest, ErrorMessage.InvalidClientRequest);
        }

        var newClaims = GetClaimsFromUser(user);

        var newAccessToken = GenerateAccessToken(newClaims);
        var newRefreshToken = GenerateRefreshToken();

        user.UserToken.RefreshToken = newRefreshToken;

        userRepository.Update(user);
        await userRepository.SaveChangesAsync(cancellationToken);

        var token = new TokenDto()
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
        };

        return DataResult<TokenDto>.Success(token);
    }

    /// <summary>
    /// Получение ClaimsPrincipal из исчезающего токена
    /// </summary>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    private ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken)
    {
        var tokenValidationParameters = new TokenValidationParameters()
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey)),
            ValidateLifetime = true,
            ValidAudience = _audience,
            ValidIssuer = _issuer
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var claimsPrincipal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new SecurityTokenException(ErrorMessage.InvalidToken);
        }
        return claimsPrincipal;
    }

}
