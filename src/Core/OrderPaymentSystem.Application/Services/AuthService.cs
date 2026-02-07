using Microsoft.Extensions.Logging;
using OrderPaymentSystem.Application.DTOs.Auth;
using OrderPaymentSystem.Application.DTOs.Token;
using OrderPaymentSystem.Application.Interfaces.Auth;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Application.Interfaces.Services;
using OrderPaymentSystem.Application.Specifications;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Shared.Result;
using System.Security.Claims;

namespace OrderPaymentSystem.Application.Services;

/// <summary>
/// Сервис авторизации и аутентификации
/// </summary>
public class AuthService : IAuthService
{
    private const int TokenLifeTimeInDays = 7;

    private readonly ILogger<AuthService> _logger;
    private readonly IUserTokenService _userTokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    /// <summary>
    /// Конструктор сервиса авторизации и аутентификации
    /// </summary>
    /// <param name="userTokenService">Сервис для работы с JWT-токенами</param>
    /// <param name="unitOfWork">Сервис для работы с транзакциями</param>
    /// <param name="passwordHasher">Сервис для хэширования паролей</param>
    /// <param name="cacheService">Сервис для работы с кэшем</param>
    public AuthService(
        ILogger<AuthService> logger,
        IUserTokenService userTokenService,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher)
    {
        _logger = logger;
        _userTokenService = userTokenService;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    /// <inheritdoc/>
    public async Task<DataResult<TokenDto>> LoginAsync(LoginUserDto dto, CancellationToken ct = default)
    {
        throw new Exception("Test elastic");

        var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(UserSpecs.ByLogin(dto.Login).ForAuth(), ct);
        if (user == null || !_passwordHasher.Verify(dto.Password, user.PasswordHash))
        {
            return DataResult<TokenDto>.Failure(DomainErrors.User.InvalidCredentials());
        }

        var claims = _userTokenService.GetClaimsFromUser(user);

        var userRoles = claims.Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToArray();

        var accessToken = _userTokenService.GenerateAccessToken(claims);
        var refreshToken = _userTokenService.GenerateRefreshToken();
        var refreshTokenExpire = DateTime.UtcNow.AddDays(TokenLifeTimeInDays);

        if (user.UserToken == null)
        {
            var newUserToken = UserToken.Create(user.Id, refreshToken, refreshTokenExpire);

            await _unitOfWork.UserToken.CreateAsync(newUserToken, ct);
        }
        else
        {
            user.UserToken.UpdateRefreshTokenData(refreshToken, refreshTokenExpire);

            _unitOfWork.UserToken.Update(user.UserToken);
        }

        await _unitOfWork.SaveChangesAsync(ct);

        var result = new TokenDto()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };

        return DataResult<TokenDto>.Success(result);
    }

    /// <inheritdoc/>
    public async Task<BaseResult> RegisterAsync(RegisterUserDto dto, CancellationToken ct = default)
    {
        var exists = await _unitOfWork.Users.AnyAsync(UserSpecs.ByLogin(dto.Login), ct);

        if (exists)
        {
            return BaseResult.Failure(DomainErrors.User.AlreadyExist(dto.Login));
        }

        await using var transaction = await _unitOfWork.BeginTransactionAsync(ct);

        try
        {
            var user = User.Create(
                dto.Login,
                _passwordHasher.Hash(dto.Password)
            );

            await _unitOfWork.Users.CreateAsync(user, ct);

            await _unitOfWork.SaveChangesAsync(ct);

            var roleId = await _unitOfWork.Roles.GetValueAsync(
                RoleSpecs.ByName(Role.DefaultUserRoleName),
                x => x.Id,
                ct);

            if (roleId == 0)
            {
                await transaction.RollbackAsync(ct);
                _logger.LogError("default User role not found during registration for user: {Login}", dto.Login);

                return BaseResult.Failure(DomainErrors.Role.NotFoundByName(Role.DefaultUserRoleName));
            }

            var userRole = UserRole.Create(user.Id, roleId);
            await _unitOfWork.UserRoles.CreateAsync(userRole, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            await transaction.CommitAsync(ct);

            return BaseResult.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);
            _logger.LogError(ex, "Error during registration for user: {Login}", dto.Login);

            return BaseResult.Failure(DomainErrors.General.InternalServerError());
        }
    }
}
