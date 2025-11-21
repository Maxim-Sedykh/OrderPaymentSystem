using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.Domain.Dto.Role;
using OrderPaymentSystem.Domain.Dto.UserRole;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces.Databases;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Interfaces.Validators;
using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Application.Services;

/// <summary>
/// Сервис для работы с ролями
/// </summary>
public class RoleService : IRoleService
{
    private readonly IBaseRepository<Role> _roleRepository;
    private readonly IBaseRepository<User> _userRepository;
    private readonly IBaseRepository<UserRole> _userRoleRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRoleValidator _roleValidator;
    private readonly ILogger<RoleService> _logger;

    public RoleService(
        IBaseRepository<Role> roleRepository,
        IBaseRepository<User> userRepository,
        IBaseRepository<UserRole> userRoleRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IRoleValidator roleValidator,
        ILogger<RoleService> logger)
    {
        _roleRepository = roleRepository;
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _roleValidator = roleValidator;
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
            return DataResult<UserRoleDto>.Failure((int)ErrorCodes.UserNotFound, ErrorMessage.UserNotFound);
        }

        var userRoles = await GetUserRoleNamesAsync(user.Id, cancellationToken);

        if (userRoles.Contains(dto.RoleName))
        {
            return DataResult<UserRoleDto>.Failure((int)ErrorCodes.UserAlreadyExistThisRole, ErrorMessage.UserAlreadyExistThisRole);
        }

        var role = await _roleRepository.GetQueryable()
            .FirstOrDefaultAsync(x => x.Name == dto.RoleName, cancellationToken);

        if (role == null)
        {
            return DataResult<UserRoleDto>.Failure((int)ErrorCodes.RoleNotFound, ErrorMessage.RoleNotFound);
        }

        var userRole = new UserRole
        {
            RoleId = role.Id,
            UserId = user.Id
        };

        await _userRoleRepository.CreateAsync(userRole, cancellationToken);
        await _userRoleRepository.SaveChangesAsync(cancellationToken);

        return DataResult<UserRoleDto>.Success(new UserRoleDto(user.Login, role.Name));
    }

    /// <inheritdoc/>
    public async Task<DataResult<RoleDto>> CreateRoleAsync(
        CreateRoleDto dto,
        CancellationToken cancellationToken = default)
    {
        var roleExists = await _roleRepository.GetQueryable()
            .AsNoTracking()
            .AnyAsync(x => x.Name == dto.Name, cancellationToken);

        if (roleExists)
        {
            return DataResult<RoleDto>.Failure((int)ErrorCodes.RoleAlreadyExist, ErrorMessage.RoleAlreadyExist);
        }

        var role = new Role
        {
            Name = dto.Name,
        };

        await _roleRepository.CreateAsync(role, cancellationToken);
        await _roleRepository.SaveChangesAsync(cancellationToken);

        return DataResult<RoleDto>.Success(_mapper.Map<RoleDto>(role));
    }

    /// <inheritdoc/>
    public async Task<DataResult<RoleDto>> DeleteRoleAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetQueryable()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (role == null)
        {
            return DataResult<RoleDto>.Failure((int)ErrorCodes.RoleNotFound, ErrorMessage.RoleNotFound);
        }

        _roleRepository.Remove(role);
        await _roleRepository.SaveChangesAsync(cancellationToken);

        return DataResult<RoleDto>.Success(_mapper.Map<RoleDto>(role));
    }

    /// <inheritdoc/>
    public async Task<DataResult<RoleDto>> UpdateRoleAsync(
        RoleDto dto,
        CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetQueryable()
            .FirstOrDefaultAsync(x => x.Id == dto.Id, cancellationToken);

        if (role == null)
        {
            return DataResult<RoleDto>.Failure((int)ErrorCodes.RoleNotFound, ErrorMessage.RoleNotFound);
        }

        if (role.Name == dto.Name)
        {
            return DataResult<RoleDto>.Failure((int)ErrorCodes.NoChangesFound, ErrorMessage.NoChangesFound);
        }

        role.Name = dto.Name;
        var updatedRole = _roleRepository.Update(role);
        await _roleRepository.SaveChangesAsync(cancellationToken);

        return DataResult<RoleDto>.Success(_mapper.Map<RoleDto>(updatedRole));
    }

    /// <inheritdoc/>
    public async Task<DataResult<UserRoleDto>> DeleteRoleForUserAsync(
        DeleteUserRoleDto dto,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetQueryable()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Login == dto.Login, cancellationToken);

        var role = user?.Roles.FirstOrDefault(x => x.Id == dto.RoleId);

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
    public async Task<DataResult<UserRoleDto>> UpdateRoleForUserAsync(
        UpdateUserRoleDto dto,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetQueryable()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Login == dto.Login, cancellationToken);

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

            var newUserRole = new UserRole
            {
                UserId = user!.Id,
                RoleId = newRole!.Id,
            };

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

    /// <inheritdoc/>
    public async Task<CollectionResult<RoleDto>> GetAllRolesAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _roleRepository.GetQueryable()
            .AsNoTracking()
            .Select(x => new RoleDto(x.Id, x.Name))
            .ToArrayAsync(cancellationToken);

        if (roles.Length == 0)
        {
            return CollectionResult<RoleDto>.Failure((int)ErrorCodes.RolesNotFound, ErrorMessage.RolesNotFound);
        }

        return CollectionResult<RoleDto>.Success(roles);
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