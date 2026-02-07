using MapsterMapper;
using Microsoft.Extensions.Logging;
using Moq;
using OrderPaymentSystem.Application.DTOs.Role;
using OrderPaymentSystem.Application.Interfaces.Cache;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Application.Services;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Errors;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace OrderPaymentSystem.UnitTests.ServiceTests
{
    public class RoleServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<ILogger<RoleService>> _loggerMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly RoleService _roleService;

        private readonly Mock<IRoleRepository> _roleRepositoryMock;

        public RoleServiceTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<RoleService>>();
            _cacheServiceMock = new Mock<ICacheService>();
            _mapperMock = new Mock<IMapper>();

            _roleRepositoryMock = new Mock<IRoleRepository>();
            _uowMock.Setup(uow => uow.Roles).Returns(_roleRepositoryMock.Object);
            _uowMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            _roleService = new RoleService(_uowMock.Object, _loggerMock.Object, _cacheServiceMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task CreateAsync_NewRole_ShouldCreateRoleAndInvalidateCache()
        {
            // Arrange
            var createDto = new CreateRoleDto { Name = "NewRole" };
            var role = Role.Create(createDto.Name);
            role.Id = 1; // Пример ID
            var roleDto = new RoleDto { Id = role.Id, Name = role.Name };

            _roleRepositoryMock.Setup(r => r.AnyAsync(It.IsAny<Specification<Role>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false); // Роль не существует
            _roleRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Role>(), It.IsAny<CancellationToken>()));
            _cacheServiceMock.Setup(cs => cs.RemoveAsync(CacheKeys.Role.All, It.IsAny<CancellationToken>()));
            _mapperMock.Setup(m => m.Map<RoleDto>(It.IsAny<Role>())).Returns(roleDto);

            // Act
            var result = await _roleService.CreateAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Name.Should().Be(createDto.Name);

            _roleRepositoryMock.Verify(r => r.CreateAsync(It.Is<Role>(role => role.Name == createDto.Name), It.IsAny<CancellationToken>()), Times.Once);
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _cacheServiceMock.Verify(cs => cs.RemoveAsync(CacheKeys.Role.All, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_RoleAlreadyExists_ShouldReturnFailure()
        {
            // Arrange
            var createDto = new CreateRoleDto { Name = "ExistingRole" };
            _roleRepositoryMock.Setup(r => r.AnyAsync(It.IsAny<Specification<Role>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true); // Роль существует

            // Act
            var result = await _roleService.CreateAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.Role.AlreadyExists(createDto.Name));
            _roleRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Role>(), It.IsAny<CancellationToken>()), Times.Never);
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteByIdAsync_ValidId_ShouldDeleteRoleAndInvalidateCache()
        {
            // Arrange
            var roleId = 1;
            var role = Role.Create("RoleToDelete");
            role.Id = roleId;

            _roleRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Specification<Role>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(role);
            _roleRepositoryMock.Setup(r => r.Remove(It.IsAny<Role>()));
            _cacheServiceMock.Setup(cs => cs.RemoveAsync(CacheKeys.Role.All, It.IsAny<CancellationToken>()));

            // Act
            var result = await _roleService.DeleteByIdAsync(roleId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _roleRepositoryMock.Verify(r => r.Remove(role), Times.Once);
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _cacheServiceMock.Verify(cs => cs.RemoveAsync(CacheKeys.Role.All, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteByIdAsync_NotFound_ShouldReturnFailure()
        {
            // Arrange
            var roleId = 99;
            _roleRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Specification<Role>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Role)null);

            // Act
            var result = await _roleService.DeleteByIdAsync(roleId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.Role.NotFoundById(roleId));
            _roleRepositoryMock.Verify(r => r.Remove(It.IsAny<Role>()), Times.Never);
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ValidNameChange_ShouldUpdateRoleAndInvalidateCache()
        {
            // Arrange
            var roleId = 1;
            var updateDto = new UpdateRoleDto { Name = "UpdatedRoleName" };
            var role = Role.Create("OldRoleName");
            role.Id = roleId;

            _roleRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Specification<Role>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(role);
            _mapperMock.Setup(m => m.Map<RoleDto>(It.IsAny<Role>())).Returns(new RoleDto { Id = roleId, Name = updateDto.Name });
            _cacheServiceMock.Setup(cs => cs.RemoveAsync(CacheKeys.Role.All, It.IsAny<CancellationToken>()));

            // Act
            var result = await _roleService.UpdateAsync(roleId, updateDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            role.Name.Should().Be(updateDto.Name); // Проверяем, что доменная сущность обновилась
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _cacheServiceMock.Verify(cs => cs.RemoveAsync(CacheKeys.Role.All, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_NoNameChange_ShouldReturnFailure()
        {
            // Arrange
            var roleId = 1;
            var roleName = "SameRoleName";
            var updateDto = new UpdateRoleDto { Name = roleName };
            var role = Role.Create(roleName);
            role.Id = roleId;

            _roleRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Specification<Role>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(role);

            // Act
            var result = await _roleService.UpdateAsync(roleId, updateDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.General.NoChanges());
            _roleRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Role>()), Times.Never); // Убедимся, что не было попытки обновления
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            _cacheServiceMock.Verify(cs => cs.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetAllAsync_RolesExist_ShouldReturnCollectionFromCache()
        {
            // Arrange
            var rolesDto = new List<RoleDto> { new RoleDto { Id = 1, Name = "Admin" }, new RoleDto { Id = 2, Name = "User" } };

            _cacheServiceMock.Setup(cs => cs.GetOrCreateAsync(CacheKeys.Role.All, It.IsAny<Func<CancellationToken, Task<List<RoleDto>>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(rolesDto);

            // Act
            var result = await _roleService.GetAllAsync();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().HaveCount(2);
            _cacheServiceMock.Verify(cs => cs.GetOrCreateAsync(CacheKeys.Role.All, It.IsAny<Func<CancellationToken, Task<List<RoleDto>>>>(), It.IsAny<CancellationToken>()), Times.Once);
            _roleRepositoryMock.Verify(r => r.GetListProjectedAsync<RoleDto>(It.IsAny<Specification<Role>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetAllAsync_RolesNotFound_ShouldReturnFailure()
        {
            // Arrange
            var rolesDto = new List<RoleDto>(); // Пустой список

            _cacheServiceMock.Setup(cs => cs.GetOrCreateAsync(CacheKeys.Role.All, It.IsAny<Func<CancellationToken, Task<List<RoleDto>>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(rolesDto);

            // Act
            var result = await _roleService.GetAllAsync();

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.Role.RolesNotFound());
            _loggerMock.Verify(l => l.LogWarning("Roles not found in database"), Times.Once);
        }
    }

}
