﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.Application.Validations.Validators;
using OrderPaymentSystem.Domain.Dto.Auth;
using OrderPaymentSystem.Domain.Dto.Order;
using OrderPaymentSystem.Domain.Dto.Token;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Helpers;
using OrderPaymentSystem.Domain.Interfaces.Databases;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Interfaces.Validators;
using OrderPaymentSystem.Domain.Result;
using System.Security.Claims;

namespace OrderPaymentSystem.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<UserToken> _userTokenRepository;
        private readonly IBaseRepository<Role> _roleRepository;
        private readonly IUserTokenService _userTokenService;
        private readonly IMapper _mapper;
        private readonly IAuthValidator _authValidator;

        public AuthService(IBaseRepository<User> userRepository, IMapper mapper, IUserTokenService userTokenService,
            IBaseRepository<UserToken> userTokenRepository, IBaseRepository<Role> roleRepository, IUnitOfWork unitOfWork,
            IAuthValidator authValidator)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _userTokenService = userTokenService;
            _userTokenRepository = userTokenRepository;
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
            _authValidator = authValidator;
        }

        /// <inheritdoc/>
        public async Task<BaseResult<TokenDto>> Login(LoginUserDto dto)
        {
            var user = await _userRepository.GetAll()
                .Include(x => x.Roles)
                .FirstOrDefaultAsync(x => x.Login == dto.Login);

            var validateLoginResult = _authValidator.ValidateLogin(user, enteredPassword : dto.Password);
            if (!validateLoginResult.IsSuccess)
            {
                return new BaseResult<TokenDto>()
                {
                    ErrorMessage = validateLoginResult.ErrorMessage,
                    ErrorCode = validateLoginResult.ErrorCode
                };
            }

            var userRoles = user.Roles;
            var claims = userRoles.Select(x => new Claim(ClaimTypes.Role, x.Name)).ToList();
            claims.Add(new Claim(ClaimTypes.Name, user.Login));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, Convert.ToString(user.Id)));

            var accessToken = _userTokenService.GenerateAccessToken(claims);
            var userToken = await _userTokenRepository.GetAll().FirstOrDefaultAsync(x => x.UserId == user.Id);
            var refreshToken = _userTokenService.GenerateRefreshToken();

            if (userToken == null)
            {
                userToken = new UserToken()
                {
                    UserId = user.Id,
                    RefreshToken = refreshToken,
                    RefreshTokenExpireTime = DateTime.UtcNow.AddDays(7)
                };

                await _userTokenRepository.CreateAsync(userToken);
            }
            else
            {
                userToken.RefreshToken = refreshToken;
                userToken.RefreshTokenExpireTime = DateTime.UtcNow.AddDays(7);

                _userTokenRepository.Update(userToken);

                await _userTokenRepository.SaveChangesAsync();
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

            var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Login == dto.Login);

            if (user == null)
            {
                return new BaseResult<UserDto>()
                {
                    ErrorCode = (int)ErrorCodes.UserNotFound,
                    ErrorMessage = ErrorMessage.UserNotFound
                };
            }

            var hashUserPassword = HashPasswordHelper.HashPassword(dto.Password);

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    user = new User()
                    {
                        Login = dto.Login,
                        Password = hashUserPassword,
                    };
                    await _unitOfWork.Users.CreateAsync(user);

                    await _unitOfWork.SaveChangesAsync();

                    Basket userBasket = new Basket()
                    {
                        UserId = user.Id,
                    };

                    await _unitOfWork.Baskets.CreateAsync(userBasket);

                    var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Name == nameof(Roles.User));

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

                    await _unitOfWork.UserRoles.CreateAsync(userRole);

                    await _unitOfWork.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                }
            }

            return new BaseResult<UserDto>()
            {
                Data = _mapper.Map<UserDto>(user),
            };
        }
    }
}
