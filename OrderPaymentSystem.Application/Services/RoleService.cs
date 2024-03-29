﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.Domain.Dto.Order;
using OrderPaymentSystem.Domain.Dto.Role;
using OrderPaymentSystem.Domain.Dto.UserRole;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces.Databases;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Interfaces.Validations;
using OrderPaymentSystem.Domain.Result;
using OrderPaymentSystem.Domain.Settings;
using OrderPaymentSystem.Producer.Interfaces;

namespace OrderPaymentSystem.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseRepository<Role> _roleRepository;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<UserRole> _userRoleRepository;
        private readonly IRoleValidator _roleValidator;
        private readonly IUserValidator _userValidator;
        private readonly IMessageProducer _messageProducer;
        private readonly IOptions<RabbitMqSettings> _rabbitMqOptions;
        private readonly IMapper _mapper;

        public RoleService(IBaseRepository<Role> roleRepository, IBaseRepository<User> userRepository,
            IRoleValidator roleValidator, IBaseRepository<UserRole> userRoleRepository, IMapper mapper, IUnitOfWork unitOfWork,
            IMessageProducer messageProducer, IOptions<RabbitMqSettings> rabbitMqOptions, IUserValidator userValidator)
        {
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _roleValidator = roleValidator;
            _userRoleRepository = userRoleRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _messageProducer = messageProducer;
            _rabbitMqOptions = rabbitMqOptions;
            _userValidator = userValidator;
        }

        /// <inheritdoc/>
        public async Task<BaseResult<UserRoleDto>> AddRoleForUserAsync(UserRoleDto dto)
        {
            var user = await _userRepository.GetAll()
                .Include(x => x.Roles)
                .FirstOrDefaultAsync(x => x.Login == dto.Login);

            var result = _userValidator.ValidateOnNull(user);
            if (!result.IsSuccess)
            {
                return new BaseResult<UserRoleDto>()
                {
                    ErrorMessage = result.ErrorMessage,
                    ErrorCode = result.ErrorCode
                };
            }

            var roles = user.Roles.Select(x => x.Name).ToArray();
            if (!roles.All(x => x == dto.RoleName))
            {
                var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Name == dto.RoleName);
                if (role == null)
                {
                    return new BaseResult<UserRoleDto>
                    {
                        ErrorCode = (int)ErrorCodes.RoleNotFound,
                        ErrorMessage = ErrorMessage.RoleNotFound,
                    };
                }

                UserRole userRole = new UserRole()
                {
                    RoleId = role.Id,
                    UserId = user.Id
                };

                await _userRoleRepository.CreateAsync(userRole);
                await _userRoleRepository.SaveChangesAsync();

                _messageProducer.SendMessage(userRole, _rabbitMqOptions.Value.RoutingKey, _rabbitMqOptions.Value.ExchangeName);

                return new BaseResult<UserRoleDto>()
                {
                    Data = new UserRoleDto(user.Login, role.Name)
                };
            }

            return new BaseResult<UserRoleDto>()
            {
                ErrorCode = (int)ErrorCodes.UserAlreadyExistThisRole,
                ErrorMessage = ErrorMessage.UserAlreadyExistThisRole,
            };
        }

        /// <inheritdoc/>
        public async Task<BaseResult<RoleDto>> CreateRoleAsync(CreateRoleDto dto)
        {
            var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Name == dto.Name);
            var createRoleValidation = _roleValidator.CreateRoleValidator(role);
            if (!createRoleValidation.IsSuccess)
            {
                return new BaseResult<RoleDto>
                {
                    ErrorMessage = createRoleValidation.ErrorMessage,
                    ErrorCode = createRoleValidation.ErrorCode,
                };
            }

            role = new Role() 
            {
                Name = dto.Name,
            };

            await _roleRepository.CreateAsync(role);
            await _roleRepository.SaveChangesAsync();

            _messageProducer.SendMessage(role, _rabbitMqOptions.Value.RoutingKey, _rabbitMqOptions.Value.ExchangeName);

            return new BaseResult<RoleDto>
            {
                Data = _mapper.Map<RoleDto>(role),
            };
        }

        /// <inheritdoc/>
        public async Task<BaseResult<RoleDto>> DeleteRoleAsync(long id)
        {
            var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
            var validateRoleOnNull = _roleValidator.ValidateOnNull(role);
            if (!validateRoleOnNull.IsSuccess)
            {
                return new BaseResult<RoleDto>()
                {
                    ErrorMessage = validateRoleOnNull.ErrorMessage,
                    ErrorCode = validateRoleOnNull.ErrorCode,
                };
            }

            _roleRepository.Remove(role);
            await _roleRepository.SaveChangesAsync();

            _messageProducer.SendMessage(role, _rabbitMqOptions.Value.RoutingKey, _rabbitMqOptions.Value.ExchangeName);

            return new BaseResult<RoleDto>()
            {
                Data = _mapper.Map<RoleDto>(role),
            };
        }

        /// <inheritdoc/>
        public async Task<BaseResult<RoleDto>> UpdateRoleAsync(RoleDto dto)
        {
            var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.Id);
            var validateRoleOnNull = _roleValidator.ValidateOnNull(role);
            if (!validateRoleOnNull.IsSuccess)
            {
                return new BaseResult<RoleDto>()
                {
                    ErrorMessage = validateRoleOnNull.ErrorMessage,
                    ErrorCode = validateRoleOnNull.ErrorCode,
                };
            }

            role.Name = dto.Name;

            var updatedRole = _roleRepository.Update(role);
            await _roleRepository.SaveChangesAsync();

            _messageProducer.SendMessage(role, _rabbitMqOptions.Value.RoutingKey, _rabbitMqOptions.Value.ExchangeName);

            return new BaseResult<RoleDto>()
            {
                Data = _mapper.Map<RoleDto>(updatedRole),
            };
        }

        /// <inheritdoc/>
        public async Task<BaseResult<UserRoleDto>> DeleteRoleForUserAsync(DeleteUserRoleDto dto)
        {
            var user = await _userRepository.GetAll()
                .Include(x => x.Roles)
                .FirstOrDefaultAsync(x => x.Login == dto.Login);

            var userNullValidation = _userValidator.ValidateOnNull(user);
            if (!userNullValidation.IsSuccess)
            {
                return new BaseResult<UserRoleDto>()
                {
                    ErrorMessage = userNullValidation.ErrorMessage,
                    ErrorCode = userNullValidation.ErrorCode
                };
            }

            var role = user.Roles.FirstOrDefault(x => x.Id == dto.RoleId);
            var validateRoleOnNull = _roleValidator.ValidateOnNull(role);
            if (!validateRoleOnNull.IsSuccess)
            {
                return new BaseResult<UserRoleDto>()
                {
                    ErrorMessage = validateRoleOnNull.ErrorMessage,
                    ErrorCode = validateRoleOnNull.ErrorCode,
                };
            }

            var userRole = await _userRoleRepository.GetAll()
                .Where(x => x.RoleId == role.Id)
                .FirstOrDefaultAsync(x => x.UserId == user.Id);
            _userRoleRepository.Remove(userRole);
            await _userRoleRepository.SaveChangesAsync();

            _messageProducer.SendMessage(userRole, _rabbitMqOptions.Value.RoutingKey, _rabbitMqOptions.Value.ExchangeName);

            return new BaseResult<UserRoleDto>()
            {
                Data = new UserRoleDto(user.Login, role.Name)
            };
        }

        /// <inheritdoc/>
        public async Task<BaseResult<UserRoleDto>> UpdateRoleForUserAsync(UpdateUserRoleDto dto)
        {
            var user = await _userRepository.GetAll()
                .Include(x => x.Roles)
                .FirstOrDefaultAsync(x => x.Login == dto.Login);

            var userNullValidation = _userValidator.ValidateOnNull(user);
            if (!userNullValidation.IsSuccess)
            {
                return new BaseResult<UserRoleDto>()
                {
                    ErrorMessage = userNullValidation.ErrorMessage,
                    ErrorCode = userNullValidation.ErrorCode
                };
            }

            var role = user.Roles.FirstOrDefault(x => x.Id == dto.FromRoleId);
            var validateRoleOnNull = _roleValidator.ValidateOnNull(role);
            if (!validateRoleOnNull.IsSuccess)
            {
                return new BaseResult<UserRoleDto>()
                {
                    ErrorMessage = validateRoleOnNull.ErrorMessage,
                    ErrorCode = validateRoleOnNull.ErrorCode,
                };
            }

            var newRoleForUser = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.ToRoleId);
            var validateNewRoleOnNull = _roleValidator.ValidateOnNull(newRoleForUser);
            if (!validateRoleOnNull.IsSuccess)
            {
                return new BaseResult<UserRoleDto>()
                {
                    ErrorMessage = validateRoleOnNull.ErrorMessage,
                    ErrorCode = validateRoleOnNull.ErrorCode,
                };
            }

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var userRole = await _unitOfWork.UserRoles.GetAll()
                                            .Where(x => x.RoleId == role.Id)
                                            .FirstAsync(x => x.UserId == user.Id);

                    _unitOfWork.UserRoles.Remove(userRole);

                    var newUserRole = new UserRole()
                    {
                        UserId = user.Id,
                        RoleId = newRoleForUser.Id, 
                    };
                    await _unitOfWork.UserRoles.CreateAsync(newUserRole);
                    await _unitOfWork.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                }
            }

            return new BaseResult<UserRoleDto>()
            {
                Data = new UserRoleDto(user.Login, newRoleForUser.Name)
            };
        }

        public async Task<CollectionResult<RoleDto>> GetAllRoles()
        {
            var roles = await _roleRepository.GetAll().Select(x => new RoleDto(x.Id, x.Name)).ToArrayAsync();

            if (roles.Length == 0)
            {
                return new CollectionResult<RoleDto>()
                {
                    ErrorMessage = ErrorMessage.RolesNotFound,
                    ErrorCode = (int)ErrorCodes.RolesNotFound
                };
            }

            return new CollectionResult<RoleDto>()
            {
                Count = roles.Length,
                Data = roles
            };
        }
    }
}
