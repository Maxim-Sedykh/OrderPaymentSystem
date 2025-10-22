using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.Domain.Constants;
using OrderPaymentSystem.Domain.Dto.Role;
using OrderPaymentSystem.Domain.Dto.UserRole;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces.Cache;
using OrderPaymentSystem.Domain.Interfaces.Databases;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Interfaces.Validators;
using OrderPaymentSystem.Domain.Result;
using OrderPaymentSystem.Domain.Settings;

namespace OrderPaymentSystem.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseRepository<Role> _roleRepository;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<UserRole> _userRoleRepository;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private readonly IRoleValidator _roleValidator;

        public RoleService(IBaseRepository<Role> roleRepository, IBaseRepository<User> userRepository,
            IBaseRepository<UserRole> userRoleRepository, IMapper mapper, IUnitOfWork unitOfWork,
            ICacheService cacheService, IRoleValidator roleValidator)
        {
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _roleValidator = roleValidator;
        }

        /// <inheritdoc/>
        public async Task<BaseResult<UserRoleDto>> AddRoleForUserAsync(UserRoleDto dto)
        {
            var user = await _userRepository.GetAll()
                .Include(x => x.Roles)
                .FirstOrDefaultAsync(x => x.Login == dto.Login);

            if (user == null)
            {
                return new BaseResult<UserRoleDto>()
                {
                    ErrorCode = (int)ErrorCodes.UserNotFound,
                    ErrorMessage = ErrorMessage.UserNotFound
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

            if (role != null)
            {
                return new BaseResult<RoleDto>()
                {
                    ErrorCode = (int)ErrorCodes.RoleAlreadyExist,
                    ErrorMessage = ErrorMessage.RoleAlreadyExist
                };
            }

            role = new Role()
            {
                Name = dto.Name,
            };

            await _roleRepository.CreateAsync(role);
            await _roleRepository.SaveChangesAsync();

            return new BaseResult<RoleDto>
            {
                Data = _mapper.Map<RoleDto>(role),
            };
        }

        /// <inheritdoc/>
        public async Task<BaseResult<RoleDto>> DeleteRoleAsync(long id)
        {
            var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);

            if (role == null)
            {
                return new BaseResult<RoleDto>()
                {
                    ErrorCode = (int)ErrorCodes.RoleNotFound,
                    ErrorMessage = ErrorMessage.RoleNotFound
                };
            }

            _roleRepository.Remove(role);
            await _roleRepository.SaveChangesAsync();

            return new BaseResult<RoleDto>()
            {
                Data = _mapper.Map<RoleDto>(role),
            };
        }

        /// <inheritdoc/>
        public async Task<BaseResult<RoleDto>> UpdateRoleAsync(RoleDto dto)
        {
            var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (role == null)
            {
                return new BaseResult<RoleDto>()
                {
                    ErrorCode = (int)ErrorCodes.RoleNotFound,
                    ErrorMessage = ErrorMessage.RoleNotFound
                };
            }

            role.Name = dto.Name;

            var updatedRole = _roleRepository.Update(role);
            await _roleRepository.SaveChangesAsync();

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

            var role = user.Roles.FirstOrDefault(x => x.Id == dto.RoleId);

            var validateRoleForUserResult = _roleValidator.ValidateRoleForUser(user, role);
            if (!validateRoleForUserResult.IsSuccess)
            {
                return new BaseResult<UserRoleDto>()
                {
                    ErrorMessage = validateRoleForUserResult.ErrorMessage,
                    ErrorCode = validateRoleForUserResult.ErrorCode
                };
            }

            var userRole = await _userRoleRepository.GetAll()
                .Where(x => x.RoleId == role.Id)
                .FirstOrDefaultAsync(x => x.UserId == user.Id);

            _userRoleRepository.Remove(userRole);
            await _userRoleRepository.SaveChangesAsync();

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

            var role = user.Roles.FirstOrDefault(x => x.Id == dto.FromRoleId);

            var newRoleForUser = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.ToRoleId);

            var validateRoleForUserResult = _roleValidator.ValidateRoleForUser(user, role, newRoleForUser);
            if (!validateRoleForUserResult.IsSuccess)
            {
                return new BaseResult<UserRoleDto>()
                {
                    ErrorMessage = validateRoleForUserResult.ErrorMessage,
                    ErrorCode = validateRoleForUserResult.ErrorCode
                };
            }

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var userRole = await _unitOfWork.UserRoles
                        .GetAll()
                        .FirstAsync(x => x.UserId == user.Id && x.RoleId == role.Id);

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
            var roles = await _cacheService.GetObjectAsync(
                CacheKeys.Roles,
                async () =>
                {
                    return await _roleRepository
                        .GetAll()
                        .Select(x => new RoleDto(x.Id, x.Name))
                        .ToArrayAsync();
                });


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
