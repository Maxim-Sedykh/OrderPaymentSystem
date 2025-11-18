using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.Domain.Dto.Auth;
using OrderPaymentSystem.Domain.Dto.Token;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces.Auth;
using OrderPaymentSystem.Domain.Interfaces.Databases;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Interfaces.Validators;
using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Application.Services;

/// <summary>
/// Сервис авторизации и аутентификации
/// </summary>
/// <param name="userRepository">Репозиторий для работы с сущностью Пользователь</param>
/// <param name="mapper">Маппер объектов</param>
/// <param name="userTokenService">Сервис для работы с JWT-токенами</param>
/// <param name="userTokenRepository"></param>
/// <param name="roleRepository">Репозиторий для работы с сущностью Роль</param>
/// <param name="unitOfWork">Сервис для работы с транзакциями</param>
/// <param name="authValidator">Валидатор</param>
/// <param name="passwordHasher">Сервис для хэширования паролей</param>
public class AuthService(IBaseRepository<User> userRepository,
    IMapper mapper,
    IUserTokenService userTokenService,
    IBaseRepository<UserToken> userTokenRepository,
    IBaseRepository<Basket> basketRepository,
    IBaseRepository<Role> roleRepository,
    IBaseRepository<UserRole> userRoleRepository,
    IUnitOfWork unitOfWork,
    IAuthValidator authValidator,
    IPasswordHasher passwordHasher) : IAuthService
{
    /// <summary>
    /// Время жизни токена в днях
    /// </summary>
    private const int TokenLifeTimeInDays = 7;

    /// <inheritdoc/>
    public async Task<DataResult<TokenDto>> LoginAsync(LoginUserDto dto, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetQueryable()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Login == dto.Login, cancellationToken);

        var validateLoginResult = authValidator.ValidateLogin(user, enteredPassword: dto.Password);
        if (!validateLoginResult.IsSuccess)
        {
            return DataResult<TokenDto>.Failure(validateLoginResult.Error);
        }

        var claims = userTokenService.GetClaimsFromUser(user);

        var accessToken = userTokenService.GenerateAccessToken(claims);
        var refreshToken = userTokenService.GenerateRefreshToken();

        var userToken = await userTokenRepository.GetQueryable().FirstOrDefaultAsync(x => x.UserId == user.Id, cancellationToken);

        if (userToken == null)
        {
            userToken = new UserToken()
            {
                UserId = user.Id,
                RefreshToken = refreshToken,
                RefreshTokenExpireTime = DateTime.UtcNow.AddDays(TokenLifeTimeInDays)
            };

            await userTokenRepository.CreateAsync(userToken, cancellationToken);
        }
        else
        {
            userToken.RefreshToken = refreshToken;
            userToken.RefreshTokenExpireTime = DateTime.UtcNow.AddDays(TokenLifeTimeInDays);

            userTokenRepository.Update(userToken);

            await userTokenRepository.SaveChangesAsync(cancellationToken);
        }

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

        var user = await userRepository.GetQueryable().FirstOrDefaultAsync(x => x.Login == dto.Login, cancellationToken);

        if (user != null)
        {
            return DataResult<UserDto>.Failure((int)ErrorCodes.UserAlreadyExist, ErrorMessage.UserAlreadyExist);
        }

        using (var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken))
        {
            try
            {
                user = new User()
                {
                    Login = dto.Login,
                    Password = passwordHasher.Hash(dto.Password),
                };
                await userRepository.CreateAsync(user, cancellationToken);

                await unitOfWork.SaveChangesAsync(cancellationToken);

                var userBasket = new Basket()
                {
                    UserId = user.Id,
                };

                await basketRepository.CreateAsync(userBasket, cancellationToken);

                var role = await roleRepository.GetQueryable().FirstOrDefaultAsync(x => x.Name == nameof(Roles.User), cancellationToken);

                if (role == null)
                {
                    return DataResult<UserDto>.Failure((int)ErrorCodes.RoleNotFound, ErrorMessage.RoleNotFound);
                }

                var userRole = new UserRole()
                {
                    UserId = user.Id,
                    RoleId = role.Id
                };

                await userRoleRepository.CreateAsync(userRole, cancellationToken);

                await unitOfWork.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);
            }
        }

        return DataResult<UserDto>.Success(mapper.Map<UserDto>(user));
    }
}
