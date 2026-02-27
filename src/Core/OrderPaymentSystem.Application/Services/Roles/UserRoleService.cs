using Microsoft.Extensions.Logging;
using OrderPaymentSystem.Application.Constants;
using OrderPaymentSystem.Application.DTOs.UserRole;
using OrderPaymentSystem.Application.Interfaces.Cache;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Application.Interfaces.Services;
using OrderPaymentSystem.Application.Specifications;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Shared.Result;

namespace OrderPaymentSystem.Application.Services.Roles;

/// <summary>
/// Сервис для взаимодействия с ролями и пользователями
/// </summary>
internal class UserRoleService : IUserRoleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UserRoleService> _logger;
    private readonly ICacheService _cacheService;

    public UserRoleService(
        IUnitOfWork unitOfWork,
        ILogger<UserRoleService> logger,
        ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _cacheService = cacheService;
    }

    /// <inheritdoc/>
    public async Task<DataResult<UserRoleDto>> CreateAsync(
        Guid userId,
        string roleName,
        CancellationToken ct = default)
    {
        var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(UserSpecs.ById(userId).WithRoles(), ct);

        if (user is null)
        {
            return DataResult<UserRoleDto>.Failure(DomainErrors.User.NotFoundById(userId));
        }

        var userRoleNames = await _unitOfWork.Roles
            .GetListValuesAsync(
            RoleSpecs.ByUserId(userId),
            x => x.Name,
            ct);

        if (userRoleNames.Contains(roleName))
        {
            return DataResult<UserRoleDto>.Failure(DomainErrors.Role.UserAlreadyHasRole(roleName));
        }

        var role = await _unitOfWork.Roles.GetFirstOrDefaultAsync(RoleSpecs.ByName(roleName), ct);
        if (role is null)
        {
            return DataResult<UserRoleDto>.Failure(DomainErrors.Role.NotFoundByName(roleName));
        }

        var userRole = UserRole.Create(user.Id, role.Id);

        await _unitOfWork.UserRoles.CreateAsync(userRole, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        await _cacheService.RemoveAsync(CacheKeys.User.Roles(userId), ct);

        return DataResult<UserRoleDto>.Success(new UserRoleDto(user.Login, role.Name));
    }

    /// <inheritdoc/>
    public async Task<DataResult<UserRoleDto>> DeleteAsync(
        Guid userId,
        int roleId,
        CancellationToken ct = default)
    {
        var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(UserSpecs.ById(userId).WithRoles(), ct);
        if (user is null) return DataResult<UserRoleDto>.Failure(DomainErrors.User.NotFoundById(userId));
        var role = user.Roles.FirstOrDefault(x => x.Id == roleId);
        if (role is null) return DataResult<UserRoleDto>.Failure(DomainErrors.Role.NotFoundById(roleId));

        var userRole = await _unitOfWork.UserRoles.GetFirstOrDefaultAsync(UserRoleSpecs.ByUserIdRoleId(user.Id, role.Id), ct);
        if (userRole is null) return DataResult<UserRoleDto>.Failure(DomainErrors.Role.NotFoundByUser(user.Id));

        _unitOfWork.UserRoles.Remove(userRole);
        await _unitOfWork.SaveChangesAsync(ct);

        await _cacheService.RemoveAsync(CacheKeys.User.Roles(userId), ct);

        return DataResult<UserRoleDto>.Success(new UserRoleDto(user.Login, role.Name));
    }

    /// <inheritdoc/>
    public async Task<DataResult<UserRoleDto>> UpdateAsync(
        Guid userId,
        UpdateUserRoleDto dto,
        CancellationToken ct = default)
    {
        var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(UserSpecs.ById(userId).WithRoles(), ct);
        if (user is null)
        {
            return DataResult<UserRoleDto>.Failure(DomainErrors.User.NotFoundById(userId));
        }

        var currentRole = user.Roles.FirstOrDefault(x => x.Id == dto.FromRoleId);
        if (currentRole is null)
        {
            return DataResult<UserRoleDto>.Failure(DomainErrors.Role.UserRoleNotFound(dto.FromRoleId));
        }

        var newRole = await _unitOfWork.Roles.GetFirstOrDefaultAsync(RoleSpecs.ById(dto.ToRoleId), ct);
        if (newRole is null)
        {
            return DataResult<UserRoleDto>.Failure(DomainErrors.Role.NotFoundById(dto.ToRoleId));
        }

        if (user.Roles.Any(x => x.Id == dto.ToRoleId))
        {
            return DataResult<UserRoleDto>.Failure(DomainErrors.Role.UserAlreadyHasRole(newRole.Name));
        }

        await using var transaction = await _unitOfWork.BeginTransactionAsync(ct);

        try
        {
            var userRole = await _unitOfWork.UserRoles.GetFirstOrDefaultAsync(UserRoleSpecs.ByUserIdRoleId(user.Id, currentRole.Id), ct);
            if (userRole is null) return DataResult<UserRoleDto>.Failure(DomainErrors.Role.NotFoundByUser(user.Id));

            _unitOfWork.UserRoles.Remove(userRole);

            var newUserRole = UserRole.Create(user.Id, newRole.Id);

            await _unitOfWork.UserRoles.CreateAsync(newUserRole, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            await transaction.CommitAsync(ct);

            await _cacheService.RemoveAsync(CacheKeys.User.Roles(userId), ct);

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
        var userRoles = await _cacheService.GetOrCreateAsync(CacheKeys.User.Roles(userId),
            async (token) => await _unitOfWork.Roles.GetListValuesAsync(RoleSpecs.ByUserId(userId), x => x.Name, token),
            ct: ct);

        return CollectionResult<string>.Success(userRoles);
    }
}
