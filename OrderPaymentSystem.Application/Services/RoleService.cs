using AutoMapper;
using Microsoft.EntityFrameworkCore;
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

public class RoleService(IBaseRepository<Role> roleRepository, IBaseRepository<User> userRepository,
    IBaseRepository<UserRole> userRoleRepository, IMapper mapper, IUnitOfWork unitOfWork,
    IRoleValidator roleValidator) : IRoleService
{
    /// <inheritdoc/>
    public async Task<DataResult<UserRoleDto>> AddRoleForUserAsync(UserRoleDto dto, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetQueryable()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Login == dto.Login, cancellationToken);

        if (user == null)
        {
            return DataResult<UserRoleDto>.Failure((int)ErrorCodes.UserNotFound, ErrorMessage.UserNotFound);
        }

        var roles = user.Roles.Select(x => x.Name).ToArray();
        if (!roles.All(x => x == dto.RoleName))
        {
            var role = await roleRepository.GetQueryable().FirstOrDefaultAsync(x => x.Name == dto.RoleName, cancellationToken);
            if (role == null)
            {
                return DataResult<UserRoleDto>.Failure((int)ErrorCodes.RoleNotFound, ErrorMessage.RoleNotFound);
            }

            var userRole = new UserRole()
            {
                RoleId = role.Id,
                UserId = user.Id
            };

            await userRoleRepository.CreateAsync(userRole, cancellationToken);
            await userRoleRepository.SaveChangesAsync(cancellationToken);

            return DataResult<UserRoleDto>.Success(new UserRoleDto(user.Login, role.Name));
        }

        return DataResult<UserRoleDto>.Failure((int)ErrorCodes.UserAlreadyExistThisRole, ErrorMessage.UserAlreadyExistThisRole);
    }

    /// <inheritdoc/>
    public async Task<DataResult<RoleDto>> CreateRoleAsync(CreateRoleDto dto, CancellationToken cancellationToken = default)
    {
        var role = await roleRepository.GetQueryable().FirstOrDefaultAsync(x => x.Name == dto.Name, cancellationToken);

        if (role != null)
        {
            return DataResult<RoleDto>.Failure((int)ErrorCodes.RoleAlreadyExist, ErrorMessage.RoleAlreadyExist);
        }

        role = new Role()
        {
            Name = dto.Name,
        };

        await roleRepository.CreateAsync(role, cancellationToken);
        await roleRepository.SaveChangesAsync(cancellationToken);

        return DataResult<RoleDto>.Success(mapper.Map<RoleDto>(role));
    }

    /// <inheritdoc/>
    public async Task<DataResult<RoleDto>> DeleteRoleAsync(long id, CancellationToken cancellationToken = default)
    {
        var role = await roleRepository.GetQueryable().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (role == null)
        {
            return DataResult<RoleDto>.Failure((int)ErrorCodes.RoleNotFound, ErrorMessage.RoleNotFound);
        }

        roleRepository.Remove(role);
        await roleRepository.SaveChangesAsync(cancellationToken);

        return DataResult<RoleDto>.Success(mapper.Map<RoleDto>(role));
    }

    /// <inheritdoc/>
    public async Task<DataResult<RoleDto>> UpdateRoleAsync(RoleDto dto, CancellationToken cancellationToken = default)
    {
        var role = await roleRepository.GetQueryable().FirstOrDefaultAsync(x => x.Id == dto.Id, cancellationToken);

        if (role == null)
        {
            return DataResult<RoleDto>.Failure((int)ErrorCodes.RoleNotFound, ErrorMessage.RoleNotFound);
        }

        role.Name = dto.Name;

        var updatedRole = roleRepository.Update(role);
        await roleRepository.SaveChangesAsync(cancellationToken);

        return DataResult<RoleDto>.Success(mapper.Map<RoleDto>(updatedRole));
    }

    /// <inheritdoc/>
    public async Task<DataResult<UserRoleDto>> DeleteRoleForUserAsync(DeleteUserRoleDto dto, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetQueryable()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Login == dto.Login, cancellationToken);

        var role = user.Roles.FirstOrDefault(x => x.Id == dto.RoleId);

        var validateRoleForUserResult = roleValidator.ValidateRoleForUser(user, role);
        if (!validateRoleForUserResult.IsSuccess)
        {
            return DataResult<UserRoleDto>.Failure(validateRoleForUserResult.Error);
        }

        var userRole = await userRoleRepository.GetQueryable()
            .Where(x => x.RoleId == role.Id)
            .FirstOrDefaultAsync(x => x.UserId == user.Id, cancellationToken);

        userRoleRepository.Remove(userRole);
        await userRoleRepository.SaveChangesAsync(cancellationToken);

        return DataResult<UserRoleDto>.Success(new UserRoleDto(user.Login, role.Name));
    }

    /// <inheritdoc/>
    public async Task<DataResult<UserRoleDto>> UpdateRoleForUserAsync(UpdateUserRoleDto dto, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetQueryable()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Login == dto.Login, cancellationToken);

        var role = user.Roles.FirstOrDefault(x => x.Id == dto.FromRoleId);

        var newRoleForUser = await roleRepository.GetQueryable().FirstOrDefaultAsync(x => x.Id == dto.ToRoleId, cancellationToken);

        var validateRoleForUserResult = roleValidator.ValidateRoleForUser(user, role, newRoleForUser);
        if (!validateRoleForUserResult.IsSuccess)
        {
            return DataResult<UserRoleDto>.Failure(validateRoleForUserResult.Error);
        }

        using (var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken))
        {
            try
            {
                var userRole = await userRoleRepository
                    .GetQueryable()
                    .FirstAsync(x => x.UserId == user.Id && x.RoleId == role.Id, cancellationToken);

                userRoleRepository.Remove(userRole);

                var newUserRole = new UserRole()
                {
                    UserId = user.Id,
                    RoleId = newRoleForUser.Id,
                };
                await userRoleRepository.CreateAsync(newUserRole, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);
            }
        }

        return DataResult<UserRoleDto>.Success(new UserRoleDto(user.Login, newRoleForUser.Name));
    }

    public async Task<CollectionResult<RoleDto>> GetAllRolesAsync(CancellationToken cancellationToken = default)
    {
        var roles = await roleRepository
                    .GetQueryable()
                    .Select(x => new RoleDto(x.Id, x.Name))
                    .ToArrayAsync(cancellationToken);


        if (roles.Length == 0)
        {
            return CollectionResult<RoleDto>.Failure((int)ErrorCodes.RolesNotFound, ErrorMessage.RolesNotFound);
        }

        return CollectionResult<RoleDto>.Success(roles);
    }
}
