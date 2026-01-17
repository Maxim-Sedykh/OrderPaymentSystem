using Microsoft.Extensions.Logging;
using OrderPaymentSystem.Application.DTOs.UserRole;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Application.Interfaces.Services;
using OrderPaymentSystem.Application.Specifications;
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
        CancellationToken ct = default)
    {
        var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(UserSpecs.ById(dto.UserId).WithRoles(), ct);

        if (user == null)
        {
            return DataResult<UserRoleDto>.Failure(DomainErrors.User.NotFoundById(dto.UserId));
        }

        var userRoleIds = await _unitOfWork.Roles
            .GetListValuesAsync(
            RoleSpecs.ByUserId(dto.UserId),
            x => x.Id,
            ct);

        if (userRoleIds.Contains(dto.RoleId))
        {
            return DataResult<UserRoleDto>.Failure(DomainErrors.Role.UserAlreadyHasRole(dto.RoleId));
        }

        var role = await _unitOfWork.Roles.GetFirstOrDefaultAsync(RoleSpecs.ById(dto.RoleId), ct);
        if (role == null)
        {
            return DataResult<UserRoleDto>.Failure(DomainErrors.Role.NotFoundById(dto.RoleId));
        }

        var userRole = UserRole.Create(user.Id, role.Id);

        await _unitOfWork.UserRoles.CreateAsync(userRole, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return DataResult<UserRoleDto>.Success(new UserRoleDto(user.Login, role.Name));
    }

    /// <inheritdoc/>
    public async Task<DataResult<UserRoleDto>> DeleteAsync(
        Guid userId,
        int roleId,
        CancellationToken ct = default)
    {
        var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(UserSpecs.ById(userId).WithRoles(), ct);
        var role = user?.Roles.FirstOrDefault(x => x.Id == roleId);
        if (user == null) return DataResult<UserRoleDto>.Failure(DomainErrors.User.NotFoundById(userId));
        if (role == null) return DataResult<UserRoleDto>.Failure(DomainErrors.Role.NotFoundById(roleId));

        var userRole = await _unitOfWork.UserRoles.GetFirstOrDefaultAsync(UserRoleSpecs.ByUserIdRoleId(user.Id, role.Id), ct);

        _unitOfWork.UserRoles.Remove(userRole!);
        await _unitOfWork.SaveChangesAsync(ct);

        return DataResult<UserRoleDto>.Success(new UserRoleDto(user.Login, role.Name));
    }

    /// <inheritdoc/>
    public async Task<DataResult<UserRoleDto>> UpdateAsync(
        Guid userId,
        UpdateUserRoleDto dto,
        CancellationToken ct = default)
    {
        var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(UserSpecs.ById(userId).WithRoles(), ct);
        if (user == null)
        {
            return DataResult<UserRoleDto>.Failure(DomainErrors.User.NotFoundById(userId));
        }

        var currentRole = user.Roles.FirstOrDefault(x => x.Id == dto.FromRoleId);
        if (currentRole == null)
        {
            return DataResult<UserRoleDto>.Failure(DomainErrors.Role.UserRoleNotFound(dto.FromRoleId));
        }

        var newRole = await _unitOfWork.Roles.GetFirstOrDefaultAsync(RoleSpecs.ById(dto.ToRoleId), ct);
        if (newRole == null)
        {
            return DataResult<UserRoleDto>.Failure(DomainErrors.Role.NotFoundById(dto.ToRoleId));
        }

        if (user.Roles.Any(x => x.Id == dto.ToRoleId))
        {
            return DataResult<UserRoleDto>.Failure(DomainErrors.Role.UserAlreadyHasRole(newRole.Id));
        }

        await using var transaction = await _unitOfWork.BeginTransactionAsync(ct);

        try
        {
            var userRole = await _unitOfWork.UserRoles.GetFirstOrDefaultAsync(UserRoleSpecs.ByUserIdRoleId(user.Id, currentRole.Id), ct);

            _unitOfWork.UserRoles.Remove(userRole);

            var newUserRole = UserRole.Create(user.Id, newRole.Id);

            await _unitOfWork.UserRoles.CreateAsync(newUserRole, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            await transaction.CommitAsync(ct);

            return DataResult<UserRoleDto>.Success(new UserRoleDto(user.Login, newRole.Name));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);
            _logger.LogError(ex, "Error while updating role for user");

            return DataResult<UserRoleDto>.Failure(DomainErrors.General.InternalServerError());
        }
    }

    public async Task<CollectionResult<string>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        var userRoles = await _unitOfWork.Roles.GetListValuesAsync(
            RoleSpecs.ByUserId(userId),
            x => x.Name,
            ct);

        if (userRoles.Count == 0)
        {
            _logger.LogWarning($"User with id {userId} has no roles");

            return CollectionResult<string>.Failure(DomainErrors.Role.NotFoundByUser(userId));
        }

        return CollectionResult<string>.Success(userRoles);
    }
}
