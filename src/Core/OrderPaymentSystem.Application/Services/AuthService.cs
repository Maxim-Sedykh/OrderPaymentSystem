using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrderPaymentSystem.Application.DTOs.Auth;
using OrderPaymentSystem.Application.DTOs.Token;
using OrderPaymentSystem.Application.Interfaces.Auth;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Application.Interfaces.Services;
using OrderPaymentSystem.Application.Settings;
using OrderPaymentSystem.Application.Specifications;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Shared.Result;
using System;
using System.Security.Claims;

namespace OrderPaymentSystem.Application.Services;

/// <summary>
/// Сервис авторизации и аутентификации
/// </summary>
public class AuthService : IAuthService
{
    private readonly int _tokenLifeTimeInDays;
    private readonly string _defaultRoleName;

    private readonly ILogger<AuthService> _logger;
    private readonly IUserTokenService _userTokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// Конструктор сервиса авторизации и аутентификации
    /// </summary>
    public AuthService(
        ILogger<AuthService> logger,
        IUserTokenService userTokenService,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        TimeProvider timeProvider,
        IOptions<JwtSettings> jwtSettings,
        IOptions<RoleSettings> roleSettings)
    {
        _logger = logger;
        _userTokenService = userTokenService;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _timeProvider = timeProvider;

        _tokenLifeTimeInDays = jwtSettings.Value.RefreshTokenValidityInDays;
        _defaultRoleName = roleSettings.Value.DefaultRoleName;
    }

    /// <inheritdoc/>
    public async Task<DataResult<TokenDto>> LoginAsync(LoginUserDto dto, CancellationToken ct = default)
    {
        var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(UserSpecs.ByLogin(dto.Login).ForAuth(), ct);
        if (user == null || !_passwordHasher.Verify(dto.Password, user.PasswordHash))
        {
            return DataResult<TokenDto>.Failure(DomainErrors.User.InvalidCredentials());
        }

        var now = _timeProvider.GetUtcNow();
        var getClaimsResult = _userTokenService.GetClaimsFromUser(user);
        if (!getClaimsResult.IsSuccess)
        {
            return DataResult<TokenDto>.Failure(getClaimsResult.Error);
        }


        var accessToken = _userTokenService.GenerateAccessToken(getClaimsResult.Data);
        var refreshToken = _userTokenService.GenerateRefreshToken();
        var refreshTokenExpire = now.AddDays(_tokenLifeTimeInDays);

        if (user.UserToken == null)
        {
            var newUserToken = UserToken.Create(
                user.Id,
                refreshToken,
                refreshTokenExpire.UtcDateTime,
                now.UtcDateTime);

            await _unitOfWork.UserToken.CreateAsync(newUserToken, ct);
        }
        else
        {
            user.UserToken.UpdateRefreshTokenData(
                refreshToken,
                refreshTokenExpire.UtcDateTime,
                now.UtcDateTime);
        }

        await _unitOfWork.SaveChangesAsync(ct);

        return DataResult<TokenDto>.Success(new TokenDto()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        });
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
                RoleSpecs.ByName(_defaultRoleName),
                x => x.Id,
                ct);

            if (roleId == 0)
            {
                await transaction.RollbackAsync(ct);
                _logger.LogError("default User role not found during registration for user: {Login}", dto.Login);

                return BaseResult.Failure(DomainErrors.Role.NotFoundByName(_defaultRoleName));
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
