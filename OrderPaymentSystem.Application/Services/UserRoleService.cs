using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.Domain.Dto.UserRole;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces.Databases;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Interfaces.Validators;
using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Application.Services
{
    /// <summary>
    /// Сервис для взаимодействия с ролями и пользователями
    /// </summary>
    public class UserRoleService : IUserRoleService
    {
        private readonly IBaseRepository<UserRole> _userRoleRepository;
        private readonly IBaseRepository<Role> _roleRepository;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IRoleValidator _roleValidator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RoleService> _logger;

        public UserRoleService(IBaseRepository<UserRole> userRoleRepository,
            IBaseRepository<Role> roleRepository,
            IBaseRepository<User> userRepository,
            IRoleValidator roleValidator,
            IUnitOfWork unitOfWork,
            ILogger<RoleService> logger)
        {
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _roleValidator = roleValidator;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<DataResult<UserRoleDto>> AddRoleForUserAsync(
            UserRoleDto dto,
            CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetQueryable()
                .Include(x => x.Roles)
                .FirstOrDefaultAsync(x => x.Login == dto.Login, cancellationToken);

            if (user == null)
            {
                return DataResult<UserRoleDto>.Failure(ErrorCodes.UserNotFound, ErrorMessage.UserNotFound);
            }

            var userRoles = await GetUserRoleNamesAsync(user.Id, cancellationToken);

            if (userRoles.Contains(dto.RoleName))
            {
                return DataResult<UserRoleDto>.Failure(ErrorCodes.UserAlreadyExistThisRole, ErrorMessage.UserAlreadyExistThisRole);
            }

            var role = await _roleRepository.GetQueryable()
                .FirstOrDefaultAsync(x => x.Name == dto.RoleName, cancellationToken);

            if (role == null)
            {
                return DataResult<UserRoleDto>.Failure((int)ErrorCodes.RoleNotFound, ErrorMessage.RoleNotFound);
            }

            var userRole = UserRole.Create(user.Id, role.Id);

            await _userRoleRepository.CreateAsync(userRole, cancellationToken);
            await _userRoleRepository.SaveChangesAsync(cancellationToken);

            return DataResult<UserRoleDto>.Success(new UserRoleDto(user.Login, role.Name));
        }

        public Task<DataResult<UserRoleDto>> CreateAsync(CreateUserRoleDto dto, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task<DataResult<UserRoleDto>> DeleteAsync(
            Guid userId,
            int roleId,
            CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetQueryable()
                .Include(x => x.Roles)
                .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

            var role = user?.Roles.FirstOrDefault(x => x.Id == roleId);

            var validationResult = _roleValidator.ValidateRoleForUser(user, role);
            if (!validationResult.IsSuccess)
            {
                return DataResult<UserRoleDto>.Failure(validationResult.Error);
            }

            var userRole = await _userRoleRepository.GetQueryable()
                .FirstOrDefaultAsync(x => x.UserId == user!.Id && x.RoleId == role!.Id, cancellationToken);

            _userRoleRepository.Remove(userRole!);
            await _userRoleRepository.SaveChangesAsync(cancellationToken);

            return DataResult<UserRoleDto>.Success(new UserRoleDto(user!.Login, role!.Name));
        }

        /// <inheritdoc/>
        public async Task<DataResult<UserRoleDto>> UpdateAsync(
            Guid userId,
            UpdateUserRoleDto dto,
            CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetQueryable()
                .Include(x => x.Roles)
                .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

            var currentRole = user?.Roles.FirstOrDefault(x => x.Id == dto.FromRoleId);
            var newRole = await _roleRepository.GetQueryable()
                .FirstOrDefaultAsync(x => x.Id == dto.ToRoleId, cancellationToken);

            var validationResult = _roleValidator.ValidateRoleForUser(user, currentRole, newRole);
            if (!validationResult.IsSuccess)
            {
                return DataResult<UserRoleDto>.Failure(validationResult.Error);
            }

            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var userRole = await _userRoleRepository.GetQueryable()
                    .FirstAsync(x => x.UserId == user!.Id && x.RoleId == currentRole!.Id, cancellationToken);

                _userRoleRepository.Remove(userRole);

                var newUserRole = UserRole.Create(user.Id, newRole.Id);

                await _userRoleRepository.CreateAsync(newUserRole, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return DataResult<UserRoleDto>.Success(new UserRoleDto(user.Login, newRole.Name));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Error while updating role for user");

                return DataResult<UserRoleDto>.Failure((int)ErrorCodes.InternalServerError, ErrorMessage.InternalServerError);
            }
        }

        public async Task<CollectionResult<string>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var userRoles = await GetUserRoleNamesAsync(userId, cancellationToken);

            if (userRoles.Count == 0)
            {
                return CollectionResult<string>.Failure(ErrorCodes.UserRolesNotFound, ErrorMessage.UserRolesNotFound);
            }

            return CollectionResult<string>.Success(userRoles);
        }

        /// <summary>
        /// Получает названия ролей пользователя в виде read-only коллекции
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Read-only коллекция названий ролей</returns>
        private async Task<IReadOnlyCollection<string>> GetUserRoleNamesAsync(Guid userId, CancellationToken cancellationToken)
        {
            var roleNames = await _userRoleRepository.GetQueryable()
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .Join(_roleRepository.GetQueryable(),
                      userRole => userRole.RoleId,
                      role => role.Id,
                      (userRole, role) => role.Name)
                .ToArrayAsync(cancellationToken);

            return roleNames;
        }
    }
}
