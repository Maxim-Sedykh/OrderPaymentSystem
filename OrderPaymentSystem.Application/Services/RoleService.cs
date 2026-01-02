using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.Domain.Dto.Role;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces.Databases.Repositories.Base;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Application.Services;

/// <summary>
/// Сервис для работы с ролями
/// </summary>
public class RoleService : IRoleService
{
    private readonly IBaseRepository<Role> _roleRepository;
    private readonly IMapper _mapper;

    public RoleService(
        IBaseRepository<Role> roleRepository,
        IMapper mapper)
    {
        _roleRepository = roleRepository;
        _mapper = mapper;
    }

    /// <inheritdoc/>
    public async Task<DataResult<RoleDto>> CreateAsync(
        CreateRoleDto dto,
        CancellationToken cancellationToken = default)
    {
        var roleExists = await _roleRepository.GetQueryable()
            .AnyAsync(x => x.Name == dto.Name, cancellationToken);

        if (roleExists)
        {
            return DataResult<RoleDto>.Failure(ErrorCodes.RoleAlreadyExist, ErrorMessage.RoleAlreadyExist);
        }

        var role = Role.Create(dto.Name);

        await _roleRepository.CreateAsync(role, cancellationToken);
        await _roleRepository.SaveChangesAsync(cancellationToken);

        return DataResult<RoleDto>.Success(_mapper.Map<RoleDto>(role));
    }

    /// <inheritdoc/>
    public async Task<BaseResult> DeleteByIdAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetQueryable()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (role == null)
        {
            return BaseResult.Failure(ErrorCodes.RoleNotFound, ErrorMessage.RoleNotFound);
        }

        _roleRepository.Remove(role);
        await _roleRepository.SaveChangesAsync(cancellationToken);

        return BaseResult.Success();
    }

    /// <inheritdoc/>
    public async Task<DataResult<RoleDto>> UpdateAsync(
        int id,
        UpdateRoleDto dto,
        CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetQueryable()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (role == null)
        {
            return DataResult<RoleDto>.Failure(ErrorCodes.RoleNotFound, ErrorMessage.RoleNotFound);
        }

        if (role.Name == dto.Name)
        {
            return DataResult<RoleDto>.Failure(ErrorCodes.NoChangesFound, ErrorMessage.NoChangesFound);
        }

        role.UpdateName(dto.Name);

        var updatedRole = _roleRepository.Update(role);

        await _roleRepository.SaveChangesAsync(cancellationToken);

        return DataResult<RoleDto>.Success(_mapper.Map<RoleDto>(updatedRole));
    }

    /// <inheritdoc/>
    public async Task<CollectionResult<RoleDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _roleRepository.GetQueryable()
            .Select(x => new RoleDto(x.Id, x.Name))
            .ToArrayAsync(cancellationToken);

        if (roles.Length == 0)
        {
            return CollectionResult<RoleDto>.Failure(ErrorCodes.RolesNotFound, ErrorMessage.RolesNotFound);
        }

        return CollectionResult<RoleDto>.Success(roles);
    }
}