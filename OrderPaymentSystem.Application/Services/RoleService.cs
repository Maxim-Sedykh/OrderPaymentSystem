using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.Domain.Dto.Auth;
using OrderPaymentSystem.Domain.Dto.Product;
using OrderPaymentSystem.Domain.Dto.Role;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Interfaces.Validations;
using OrderPaymentSystem.Domain.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IBaseRepository<Role> _roleRepository;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<UserRole> _userRoleRepository;
        private readonly IRoleValidator _roleValidator;
        private readonly IMapper _mapper;

        public RoleService(IBaseRepository<Role> roleRepository, IBaseRepository<User> userRepository,
            IRoleValidator roleValidator, IBaseRepository<UserRole> userRoleRepository)
        {
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _roleValidator = roleValidator;
            _userRoleRepository = userRoleRepository;
        }

        public async Task<BaseResult<UserRoleDto>> AddRoleForUserAsync(UserRoleDto dto)
        {
            var user = await _userRepository.GetAll()
                .Include(x => x.Roles)
                .FirstOrDefaultAsync(x => x.Login == dto.Login);

            if (user == null)
            {
                return new BaseResult<UserRoleDto>
                {
                    ErrorCode = (int)ErrorCodes.UserNotFound,
                    ErrorMessage = ErrorMessage.UserNotFound,
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
            return new BaseResult<RoleDto>
            {
                Data = _mapper.Map<RoleDto>(role),
            };
        }

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

            await _roleRepository.RemoveAsync(role);
            return new BaseResult<RoleDto>()
            {
                Data = _mapper.Map<RoleDto>(role),
            };
        }

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
            await _roleRepository.UpdateAsync(role);

            return new BaseResult<RoleDto>()
            {
                Data = _mapper.Map<RoleDto>(role),
            };
        }
    }
}
