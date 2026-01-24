using MapsterMapper;
using Microsoft.Extensions.Logging;
using OrderPaymentSystem.Application.Constants;
using OrderPaymentSystem.Application.DTOs.Role;
using OrderPaymentSystem.Application.Interfaces.Cache;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Application.Interfaces.Services;
using OrderPaymentSystem.Application.Specifications;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Shared.Result;

namespace OrderPaymentSystem.Application.Services;

/// <summary>
/// Сервис для работы с ролями
/// </summary>
public class RoleService : IRoleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RoleService> _logger;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;

    public RoleService(
        IUnitOfWork unitOfWork,
        ILogger<RoleService> logger,
        ICacheService cacheService,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _cacheService = cacheService;
        _mapper = mapper;
    }

    /// <inheritdoc/>
    public async Task<DataResult<RoleDto>> CreateAsync(
        CreateRoleDto dto,
        CancellationToken ct = default)
    {
        var roleExists = await _unitOfWork.Roles.AnyAsync(RoleSpecs.ByName(dto.Name), ct);

        if (roleExists)
        {
            return DataResult<RoleDto>.Failure(DomainErrors.Role.AlreadyExists(dto.Name));
        }

        var role = Role.Create(dto.Name);

        await _unitOfWork.Roles.CreateAsync(role, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        await _cacheService.RemoveAsync(CacheKeys.Role.All, ct);

        return DataResult<RoleDto>.Success(_mapper.Map<RoleDto>(role));
    }

    /// <inheritdoc/>
    public async Task<BaseResult> DeleteByIdAsync(
        int id,
        CancellationToken ct = default)
    {
        var role = await _unitOfWork.Roles.GetFirstOrDefaultAsync(RoleSpecs.ById(id), ct);

        if (role == null)
        {
            return BaseResult.Failure(DomainErrors.Role.NotFoundById(id));
        }

        _unitOfWork.Roles.Remove(role);
        await _unitOfWork.SaveChangesAsync(ct);

        await _cacheService.RemoveAsync(CacheKeys.Role.All, ct);

        return BaseResult.Success();
    }

    /// <inheritdoc/>
    public async Task<DataResult<RoleDto>> UpdateAsync(
        int id,
        UpdateRoleDto dto,
        CancellationToken ct = default)
    {
        var role = await _unitOfWork.Roles.GetFirstOrDefaultAsync(RoleSpecs.ById(id), ct);

        if (role == null)
        {
            return DataResult<RoleDto>.Failure(DomainErrors.Role.NotFoundById(id));
        }

        if (role.Name == dto.Name)
        {
            return DataResult<RoleDto>.Failure(DomainErrors.General.NoChanges());
        }

        role.UpdateName(dto.Name);

        _unitOfWork.Roles.Update(role);

        await _unitOfWork.SaveChangesAsync(ct);

        await _cacheService.RemoveAsync(CacheKeys.Role.All, ct);

        return DataResult<RoleDto>.Success(_mapper.Map<RoleDto>(role));
    }

    /// <inheritdoc/>
    public async Task<CollectionResult<RoleDto>> GetAllAsync(CancellationToken ct = default)
    {
        var roles = await _cacheService.GetOrCreateAsync(CacheKeys.Role.All,
            async (token) => await _unitOfWork.Roles.GetListProjectedAsync<RoleDto>(ct: token),
            ct: ct);

        if (roles.Count == 0)
        {
            _logger.LogWarning("Roles not found in database");

            return CollectionResult<RoleDto>.Failure(DomainErrors.Role.RolesNotFound());
        }

        return CollectionResult<RoleDto>.Success(roles);
    }
}