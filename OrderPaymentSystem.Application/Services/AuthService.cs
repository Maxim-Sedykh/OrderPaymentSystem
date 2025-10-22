using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.Domain.Dto.Auth;
using OrderPaymentSystem.Domain.Dto.Token;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces.Auth;
using OrderPaymentSystem.Domain.Interfaces.Databases;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Interfaces.Validators;
using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Application.Services
{
    public class AuthService(IBaseRepository<User> userRepository, IMapper mapper, IUserTokenService userTokenService,
            IBaseRepository<UserToken> userTokenRepository, IBaseRepository<Role> roleRepository, IUnitOfWork unitOfWork,
            IAuthValidator authValidator, IPasswordHasher passwordHasher) : IAuthService
    {

        /// <inheritdoc/>
        public async Task<BaseResult<TokenDto>> Login(LoginUserDto dto)
        {
            var user = await userRepository.GetAll()
                .Include(x => x.Roles)
                .FirstOrDefaultAsync(x => x.Login == dto.Login);

            var validateLoginResult = authValidator.ValidateLogin(user, enteredPassword: dto.Password);
            if (!validateLoginResult.IsSuccess)
            {
                return new BaseResult<TokenDto>()
                {
                    ErrorMessage = validateLoginResult.ErrorMessage,
                    ErrorCode = validateLoginResult.ErrorCode
                };
            }

            var claims = userTokenService.GetClaimsFromUser(user);

            var accessToken = userTokenService.GenerateAccessToken(claims);
            var refreshToken = userTokenService.GenerateRefreshToken();

            var userToken = await userTokenRepository.GetAll().FirstOrDefaultAsync(x => x.UserId == user.Id);

            if (userToken == null)
            {
                userToken = new UserToken()
                {
                    UserId = user.Id,
                    RefreshToken = refreshToken,
                    RefreshTokenExpireTime = DateTime.UtcNow.AddDays(7)
                };

                await userTokenRepository.CreateAsync(userToken);
            }
            else
            {
                userToken.RefreshToken = refreshToken;
                userToken.RefreshTokenExpireTime = DateTime.UtcNow.AddDays(7);

                userTokenRepository.Update(userToken);

                await userTokenRepository.SaveChangesAsync();
            }

            return new BaseResult<TokenDto>()
            {
                Data = new TokenDto()
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                }
            };
        }

        /// <inheritdoc/>
        public async Task<BaseResult<UserDto>> Register(RegisterUserDto dto)
        {
            if (dto.Password != dto.PasswordConfirm)
            {
                return new BaseResult<UserDto>()
                {
                    ErrorMessage = ErrorMessage.PasswordNotEqualsPasswordConfirm,
                    ErrorCode = (int)ErrorCodes.PasswordNotEqualsPasswordConfirm,
                };
            }

            var user = await userRepository.GetAll().FirstOrDefaultAsync(x => x.Login == dto.Login);

            if (user != null)
            {
                return new BaseResult<UserDto>()
                {
                    ErrorCode = (int)ErrorCodes.UserAlreadyExist,
                    ErrorMessage = ErrorMessage.UserAlreadyExist
                };
            }

            using (var transaction = await unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    user = new User()
                    {
                        Login = dto.Login,
                        Password = passwordHasher.Hash(dto.Password),
                    };
                    await unitOfWork.Users.CreateAsync(user);

                    await unitOfWork.SaveChangesAsync();

                    Basket userBasket = new Basket()
                    {
                        UserId = user.Id,
                    };

                    await unitOfWork.Baskets.CreateAsync(userBasket);

                    var role = await roleRepository.GetAll().FirstOrDefaultAsync(x => x.Name == nameof(Roles.User));

                    if (role == null)
                    {
                        return new BaseResult<UserDto>()
                        {
                            ErrorCode = (int)ErrorCodes.RoleNotFound,
                            ErrorMessage = ErrorMessage.RoleNotFound
                        };
                    }

                    UserRole userRole = new UserRole()
                    {
                        UserId = user.Id,
                        RoleId = role.Id
                    };

                    await unitOfWork.UserRoles.CreateAsync(userRole);

                    await unitOfWork.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                }
            }

            return new BaseResult<UserDto>()
            {
                Data = mapper.Map<UserDto>(user),
            };
        }
    }
}
