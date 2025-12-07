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
    private readonly IMapper _mapper;
    private readonly ILogger<AuthService> _logger;
    private readonly IUserTokenService _userTokenService;
    private readonly IBaseRepository<UserToken> _userTokenRepository;
    private readonly IBaseRepository<Basket> _basketRepository;
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
    /// <param name="mapper">Маппер объектов</param>
    /// <param name="userTokenService">Сервис для работы с JWT-токенами</param>
    /// <param name="userTokenRepository"></param>
    /// <param name="roleRepository">Репозиторий для работы с сущностью Роль</param>
    /// <param name="unitOfWork">Сервис для работы с транзакциями</param>
    /// <param name="authValidator">Валидатор</param>
    /// <param name="passwordHasher">Сервис для хэширования паролей</param>
    /// <param name="cacheService">Сервис для работы с кэшем</param>
    public AuthService(
        IBaseRepository<User> userRepository,
        IMapper mapper,
        ILogger<AuthService> logger,
        IUserTokenService userTokenService,
        IBaseRepository<UserToken> userTokenRepository,
        IBaseRepository<Basket> basketRepository,
        IBaseRepository<Role> roleRepository,
        IBaseRepository<UserRole> userRoleRepository,
        IUnitOfWork unitOfWork,
        IAuthValidator authValidator,
        IPasswordHasher passwordHasher,
        ICacheService cacheService)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
        _userTokenService = userTokenService;
        _userTokenRepository = userTokenRepository;
        _basketRepository = basketRepository;
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
            .FirstOrDefaultAsync(x => x.Login == dto.Login, cancellationToken);

        var validateLoginResult = _authValidator.ValidateLogin(user, enteredPassword: dto.Password);
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

        var userToken = await _userTokenRepository.GetQueryable()
            .FirstOrDefaultAsync(x => x.UserId == user.Id, cancellationToken);

        if (userToken == null)
        {
            userToken = new UserToken()
            {
                UserId = user.Id,
                RefreshToken = refreshToken,
                RefreshTokenExpireTime = refreshTokenExpire
            };

            await _userTokenRepository.CreateAsync(userToken, cancellationToken);
        }
        else
        {
            userToken.RefreshToken = refreshToken;
            userToken.RefreshTokenExpireTime = refreshTokenExpire;

            _userTokenRepository.Update(userToken);
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
    public async Task<DataResult<UserDto>> RegisterAsync(RegisterUserDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.Password != dto.PasswordConfirm)
        {
            return DataResult<UserDto>.Failure((int)ErrorCodes.PasswordNotEqualsPasswordConfirm,
                ErrorMessage.PasswordNotEqualsPasswordConfirm);
        }

        var exists = await _userRepository.GetQueryable()
            .AsNoTracking()
            .AnyAsync(x => x.Login == dto.Login, cancellationToken);

        if (exists)
        {
            return DataResult<UserDto>.Failure((int)ErrorCodes.UserAlreadyExist, ErrorMessage.UserAlreadyExist);
        }

        User user = null;

        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            user = new User()
            {
                Login = dto.Login,
                Password = _passwordHasher.Hash(dto.Password),
            };
            await _userRepository.CreateAsync(user, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var userBasket = new Basket()
            {
                UserId = user.Id,
            };
            await _basketRepository.CreateAsync(userBasket, cancellationToken);

            var roleId = await _roleRepository.GetQueryable()
                .Where(x => x.Name == nameof(Roles.User))
                .Select(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (roleId == 0)
            {
                await transaction.RollbackAsync(cancellationToken);

                return DataResult<UserDto>.Failure((int)ErrorCodes.RoleNotFound, ErrorMessage.RoleNotFound);
            }

            var userRole = new UserRole()
            {
                UserId = user.Id,
                RoleId = roleId
            };

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

            var resultDto = _mapper.Map<UserDto>(user);

            return DataResult<UserDto>.Success(resultDto);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Error during registration for user: {Login}", dto.Login);

            throw;
        }
    }
}
