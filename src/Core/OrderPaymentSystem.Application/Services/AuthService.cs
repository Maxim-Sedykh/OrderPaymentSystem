using Microsoft.Extensions.Logging;
using OrderPaymentSystem.Application.DTOs.Auth;
using OrderPaymentSystem.Application.DTOs.Token;
using OrderPaymentSystem.Application.Interfaces.Auth;
using OrderPaymentSystem.Application.Interfaces.Cache;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Application.Interfaces.Services;
using OrderPaymentSystem.Application.Interfaces.Validators;
using OrderPaymentSystem.Domain.Constants;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Domain.Resources;
using OrderPaymentSystem.Shared.Result;
using System.Security.Claims;

namespace OrderPaymentSystem.Application.Services;

/// <summary>
/// Сервис авторизации и аутентификации
/// </summary>
public class AuthService : IAuthService
{
    private const int TokenLifeTimeInDays = 7;
    private const string DefaultUserRoleName = "User";

    private readonly ILogger<AuthService> _logger;
    private readonly IUserTokenService _userTokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthValidator _authValidator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ICacheService _cacheService;

    /// <summary>
    /// Конструктор сервиса авторизации и аутентификации
    /// </summary>
    /// <param name="userTokenService">Сервис для работы с JWT-токенами</param>
    /// <param name="unitOfWork">Сервис для работы с транзакциями</param>
    /// <param name="authValidator">Валидатор</param>
    /// <param name="passwordHasher">Сервис для хэширования паролей</param>
    /// <param name="cacheService">Сервис для работы с кэшем</param>
    public AuthService(
        ILogger<AuthService> logger,
        IUserTokenService userTokenService,
        IUnitOfWork unitOfWork,
        IAuthValidator authValidator,
        IPasswordHasher passwordHasher,
        ICacheService cacheService)
    {
        _logger = logger;
        _userTokenService = userTokenService;
        _unitOfWork = unitOfWork;
        _authValidator = authValidator;
        _passwordHasher = passwordHasher;
        _cacheService = cacheService;
    }

    /// <inheritdoc/>
    public async Task<DataResult<TokenDto>> LoginAsync(LoginUserDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetWithRolesAndTokenByLoginAsync(dto.Login, cancellationToken);

        var validateLoginResult = _authValidator.ValidateLogin(user, dto.Password);
        if (!validateLoginResult.IsSuccess)
            return DataResult<TokenDto>.Failure(validateLoginResult.Error);

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

            await _unitOfWork.UserToken.CreateAsync(newUserToken, cancellationToken);
        }
        else
        {
            user.UserToken.UpdateRefreshTokenData(refreshToken, refreshTokenExpire);

            _unitOfWork.UserToken.Update(user.UserToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = new TokenDto()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };

        return DataResult<TokenDto>.Success(result);
    }

    /// <inheritdoc/>
    public async Task<BaseResult> RegisterAsync(RegisterUserDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.Password != dto.PasswordConfirm)
        {
            return BaseResult.Failure(DomainErrors.User.PasswordMismatch());
        }

        var exists = await _unitOfWork.Users.ExistsByLoginAsync(dto.Login, cancellationToken);

        if (exists)
        {
            return BaseResult.Failure(DomainErrors.User.AlreadyExist(dto.Login));
        }

        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        User user;
        try
        {
            user = User.Create(
                dto.Login,
                _passwordHasher.Hash(dto.Password)
            );

            await _unitOfWork.Users.CreateAsync(user, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var roleId = await _unitOfWork.Roles.GetRoleIdByNameAsync(DefaultUserRoleName);

            if (roleId == 0)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError("default User role not found during registration for user: {Login}", dto.Login);

                return BaseResult.Failure(DomainErrors.Role.NotFoundByName(DefaultUserRoleName));
            }

            var userRole = UserRole.Create(user.Id, roleId);
            await _unitOfWork.UserRoles.CreateAsync(userRole, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return BaseResult.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Error during registration for user: {Login}", dto.Login);

            return BaseResult.Failure(DomainErrors.General.InternalServerError());
        }
    }
}
