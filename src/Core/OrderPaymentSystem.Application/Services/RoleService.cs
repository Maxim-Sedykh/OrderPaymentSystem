using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderPaymentSystem.Application.DTOs.Role;
using OrderPaymentSystem.Application.Extensions;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Application.Interfaces.Services;
using OrderPaymentSystem.Domain.Constants;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Domain.Resources;
using OrderPaymentSystem.Shared.Result;

namespace OrderPaymentSystem.Application.Services;

/// <summary>
/// Сервис для работы с ролями
/// </summary>
public class RoleService : IRoleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<RoleService> _logger;

    public RoleService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<RoleService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<DataResult<RoleDto>> CreateAsync(
        CreateRoleDto dto,
        CancellationToken cancellationToken = default)
    {
        var roleExists = await _unitOfWork.Roles.GetQueryable()
            .AnyAsync(x => x.Name == dto.Name, cancellationToken);

        if (roleExists)
        {
            return DataResult<RoleDto>.Failure(DomainErrors.Role.AlreadyExists(dto.Name));
        }

        var role = Role.Create(dto.Name);

        await _unitOfWork.Roles.CreateAsync(role, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return DataResult<RoleDto>.Success(_mapper.Map<RoleDto>(role));
    }

    /// <inheritdoc/>
    public async Task<BaseResult> DeleteByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var role = await _unitOfWork.Roles.GetByIdAsync(id, cancellationToken);

        if (role == null)
        {
            return BaseResult.Failure(DomainErrors.Role.NotFoundById(id));
        }

        _unitOfWork.Roles.Remove(role);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return BaseResult.Success();
    }

    /// <inheritdoc/>
    public async Task<DataResult<RoleDto>> UpdateAsync(
        int id,
        UpdateRoleDto dto,
        CancellationToken cancellationToken = default)
    {
        var role = await _unitOfWork.Roles.GetByIdAsync(id, cancellationToken);

        if (role == null)
        {
            return DataResult<RoleDto>.Failure(DomainErrors.Role.NotFoundById(id));
        }

        if (role.Name == dto.Name)
        {
            return DataResult<RoleDto>.Failure(DomainErrors.General.NoChanges());
        }

        role.UpdateName(dto.Name);

        var updatedRole = _unitOfWork.Roles.Update(role);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return DataResult<RoleDto>.Success(_mapper.Map<RoleDto>(updatedRole));
    }

    /// <inheritdoc/>
    public async Task<CollectionResult<RoleDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _unitOfWork.Roles.GetAllQuery()
            .AsProjected<Role, RoleDto>(_mapper)
            .ToArrayAsync(cancellationToken);

        if (roles.Length == 0)
        {
            _logger.LogWarning("Roles not found in database");

            return CollectionResult<RoleDto>.Failure(DomainErrors.Role.RolesNotFound());
        }

        return CollectionResult<RoleDto>.Success(roles);
    }
}