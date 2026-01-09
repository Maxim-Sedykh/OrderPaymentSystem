using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderPaymentSystem.Application.DTOs.UserRole;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Application.Interfaces.Services;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Shared.Result;

namespace OrderPaymentSystem.Application.Services;

/// <summary>
/// Сервис для взаимодействия с ролями и пользователями
/// </summary>
public class UserRoleService : IUserRoleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RoleService> _logger;

    public UserRoleService(
        IUnitOfWork unitOfWork,
        ILogger<RoleService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<DataResult<UserRoleDto>> CreateAsync(
        CreateUserRoleDto dto,
        CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdWithRolesAsync(dto.UserId, cancellationToken);

        if (user == null)
        {
            return DataResult<UserRoleDto>.Failure(DomainErrors.User.NotFoundById(dto.UserId));
        }

        var userRoleIds = await _unitOfWork.Roles.GetByUserIdQuery(user.Id)
            .Select(x => x.Id)
            .ToHashSetAsync(cancellationToken);
        if (userRoleIds.Contains(dto.RoleId))
        {
            return DataResult<UserRoleDto>.Failure(DomainErrors.Role.UserAlreadyHasRole(dto.RoleId));
        }

        var role = await _unitOfWork.Roles.GetByIdAsync(dto.RoleId, cancellationToken);
        if (role == null)
        {
            return DataResult<UserRoleDto>.Failure(DomainErrors.Role.NotFoundById(dto.RoleId));
        }

        var userRole = UserRole.Create(user.Id, role.Id);

        await _unitOfWork.UserRoles.CreateAsync(userRole, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return DataResult<UserRoleDto>.Success(new UserRoleDto(user.Login, role.Name));
    }

    /// <inheritdoc/>
    public async Task<DataResult<UserRoleDto>> DeleteAsync(
        Guid userId,
        int roleId,
        CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdWithRolesAsync(userId, cancellationToken);
        var role = user?.Roles.FirstOrDefault(x => x.Id == roleId);
        if (user == null) return DataResult<UserRoleDto>.Failure(DomainErrors.User.NotFoundById(userId));
        if (role == null) return DataResult<UserRoleDto>.Failure(DomainErrors.Role.NotFoundById(roleId));

        var userRole = await _unitOfWork.UserRoles.GetByUserIdAndRoleIdAsync(user.Id, role.Id, cancellationToken);

        _unitOfWork.UserRoles.Remove(userRole!);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return DataResult<UserRoleDto>.Success(new UserRoleDto(user.Login, role.Name));
    }

    /// <inheritdoc/>
    public async Task<DataResult<UserRoleDto>> UpdateAsync(
        Guid userId,
        UpdateUserRoleDto dto,
        CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdWithRolesAsync(userId, cancellationToken);
        if (user == null)
        {
            return DataResult<UserRoleDto>.Failure(DomainErrors.User.NotFoundById(userId));
        }

        var currentRole = user.Roles.FirstOrDefault(x => x.Id == dto.FromRoleId);
        if (currentRole == null)
        {
            return DataResult<UserRoleDto>.Failure(DomainErrors.Role.UserRoleNotFound(dto.FromRoleId));
        }

        var newRole = await _unitOfWork.Roles.GetByIdAsync(dto.ToRoleId, cancellationToken);
        if (newRole == null)
        {
            return DataResult<UserRoleDto>.Failure(DomainErrors.Role.NotFoundById(dto.ToRoleId));
        }

        if (user.Roles.Any(x => x.Id == dto.ToRoleId))
        {
            return DataResult<UserRoleDto>.Failure(DomainErrors.Role.UserAlreadyHasRole(newRole.Id));
        }

        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var userRole = await _unitOfWork.UserRoles.GetByUserIdAndRoleIdAsync(user.Id, currentRole.Id, cancellationToken);

            _unitOfWork.UserRoles.Remove(userRole);

            var newUserRole = UserRole.Create(user.Id, newRole.Id);

            await _unitOfWork.UserRoles.CreateAsync(newUserRole, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return DataResult<UserRoleDto>.Success(new UserRoleDto(user.Login, newRole.Name));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Error while updating role for user");

            return DataResult<UserRoleDto>.Failure(DomainErrors.General.InternalServerError());
        }
    }

    public async Task<CollectionResult<string>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userRoles = await _unitOfWork.Roles.GetByUserIdQuery(userId)
            .Select(x => x.Name)
            .ToArrayAsync(cancellationToken);

        if (userRoles.Length == 0)
        {
            _logger.LogWarning($"User with id {userId} has no roles");

            return CollectionResult<string>.Failure(DomainErrors.Role.NotFoundByUser(userId));
        }

        return CollectionResult<string>.Success(userRoles);
    }
}
