using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.Domain.Constants;
using OrderPaymentSystem.Domain.Dto.Auth;
using OrderPaymentSystem.Domain.Dto.Token;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces.Auth;
using OrderPaymentSystem.Domain.Interfaces.Cache;
using OrderPaymentSystem.Domain.Interfaces.Databases;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Interfaces.Validators;
using OrderPaymentSystem.Domain.Result;
using System.Security.Claims;

namespace OrderPaymentSystem.Application.Services;

/// <summary>
/// Сервис авторизации и аутентификации
/// </summary>
public class AuthService : IAuthService
{
    private const int TokenLifeTimeInDays = 7;

    private readonly IBaseRepository<User> _userRepository;
    private readonly ILogger<AuthService> _logger;
    private readonly IUserTokenService _userTokenService;
    private readonly IBaseRepository<UserToken> _userTokenRepository;
    private readonly IBaseRepository<Role> _roleRepository;
    private readonly IBaseRepository<UserRole> _userRoleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthValidator _authValidator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ICacheService _cacheService;

    /// <summary>
    /// Конструктор сервиса авторизации и аутентификации
    /// </summary>
    /// <param name="userRepository">Репозиторий для работы с сущностью Пользователь</param>
    /// <param name="userTokenService">Сервис для работы с JWT-токенами</param>
    /// <param name="userTokenRepository"></param>
    /// <param name="roleRepository">Репозиторий для работы с сущностью Роль</param>
    /// <param name="unitOfWork">Сервис для работы с транзакциями</param>
    /// <param name="authValidator">Валидатор</param>
    /// <param name="passwordHasher">Сервис для хэширования паролей</param>
    /// <param name="cacheService">Сервис для работы с кэшем</param>
    public AuthService(
        IBaseRepository<User> userRepository,
        ILogger<AuthService> logger,
        IUserTokenService userTokenService,
        IBaseRepository<UserToken> userTokenRepository,
        IBaseRepository<Role> roleRepository,
        IBaseRepository<UserRole> userRoleRepository,
        IUnitOfWork unitOfWork,
        IAuthValidator authValidator,
        IPasswordHasher passwordHasher,
        ICacheService cacheService)
    {
        _userRepository = userRepository;
        _logger = logger;
        _userTokenService = userTokenService;
        _userTokenRepository = userTokenRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _unitOfWork = unitOfWork;
        _authValidator = authValidator;
        _passwordHasher = passwordHasher;
        _cacheService = cacheService;
    }

    /// <inheritdoc/>
    public async Task<DataResult<TokenDto>> LoginAsync(LoginUserDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetQueryable()
            .AsNoTracking()
            .Include(x => x.Roles)
            .Include(x => x.UserToken)
            .FirstOrDefaultAsync(x => x.Login == dto.Login, cancellationToken);

        var validateLoginResult = _authValidator.ValidateLogin(user, dto.Password);
        if (!validateLoginResult.IsSuccess)
            return DataResult<TokenDto>.Failure(validateLoginResult.Error);

        var claims = _userTokenService.GetClaimsFromUser(user);

        var userRoles = claims.Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToArray();

        await _cacheService.SetAsync(
            CacheKeys.UserRoles(user.Id),
            userRoles,
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) },
            cancellationToken);

        var accessToken = _userTokenService.GenerateAccessToken(claims);
        var refreshToken = _userTokenService.GenerateRefreshToken();
        var refreshTokenExpire = DateTime.UtcNow.AddDays(TokenLifeTimeInDays);

        if (user.UserToken == null)
        {
            var newUserToken = UserToken.Create(user.Id, refreshToken, refreshTokenExpire);

            await _userTokenRepository.CreateAsync(newUserToken, cancellationToken);
        }
        else
        {
            user.UserToken.UpdateRefreshTokenData(refreshToken, refreshTokenExpire);

            _userTokenRepository.Update(user.UserToken);
        }

        await _userTokenRepository.SaveChangesAsync(cancellationToken);

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
            return BaseResult.Failure((int)ErrorCodes.PasswordNotEqualsPasswordConfirm,
                ErrorMessage.PasswordNotEqualsPasswordConfirm);
        }

        var exists = await _userRepository.GetQueryable()
            .AnyAsync(x => x.Login == dto.Login, cancellationToken);

        if (exists)
        {
            return BaseResult.Failure((int)ErrorCodes.UserAlreadyExist, ErrorMessage.UserAlreadyExist);
        }

        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        User user;
        try
        {
            user = User.Create(
                dto.Login,
                _passwordHasher.Hash(dto.Password)
            );

            await _userRepository.CreateAsync(user, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var roleId = await _roleRepository.GetQueryable()
                .Where(x => x.Name == nameof(Roles.User))
                .Select(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (roleId == 0)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError("Default 'User' role not found during registration for user: {Login}", dto.Login);
                return BaseResult.Failure((int)ErrorCodes.RoleNotFound, ErrorMessage.RoleNotFound);
            }

            var userRole = UserRole.Create(user.Id, roleId);
            await _userRoleRepository.CreateAsync(userRole, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            await _cacheService.SetAsync(
                CacheKeys.UserRoles(user.Id),
                new[] 
                { 
                    nameof(Roles.User) 
                },
                new DistributedCacheEntryOptions 
                { 
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) 
                },
                cancellationToken);

            return BaseResult.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Error during registration for user: {Login}", dto.Login);

            return BaseResult.Failure((int)ErrorCodes.InternalServerError, ErrorMessage.InternalServerError);
        }
    }
}
