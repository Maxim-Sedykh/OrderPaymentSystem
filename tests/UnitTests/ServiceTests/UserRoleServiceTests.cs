using FluentAssertions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;
using OrderPaymentSystem.Application.Constants;
using OrderPaymentSystem.Application.DTOs.UserRole;
using OrderPaymentSystem.Application.Interfaces.Cache;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Application.Services;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Shared.Specifications;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Xunit;

namespace OrderPaymentSystem.UnitTests.ServiceTests
{
    public class UserRoleServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<ILogger<RoleService>> _loggerMock; // Логгер RoleService, но используется в UserRoleService
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly UserRoleService _userRoleService;

        // Моки репозиториев
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly Mock<IUserRoleRepository> _userRoleRepositoryMock;

        private readonly Mock<IDbContextTransaction> _transactionMock = new();

        public UserRoleServiceTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<RoleService>>(); // Используем тот же тип логгера, что и в сервисе
            _cacheServiceMock = new Mock<ICacheService>();

            _userRepositoryMock = new Mock<IUserRepository>();
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _userRoleRepositoryMock = new Mock<IUserRoleRepository>();

            _uowMock.Setup(uow => uow.Users).Returns(_userRepositoryMock.Object);
            _uowMock.Setup(uow => uow.Roles).Returns(_roleRepositoryMock.Object);
            _uowMock.Setup(uow => uow.UserRoles).Returns(_userRoleRepositoryMock.Object);
            _uowMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            _uowMock.Setup(uow => uow.BeginTransactionAsync(It.IsAny<CancellationToken>())).ReturnsAsync(_transactionMock.Object);

            _userRoleService = new UserRoleService(_uowMock.Object, _loggerMock.Object, _cacheServiceMock.Object);
        }

        [Fact]
        public async Task CreateAsync_NewUserRole_ShouldCreateAndReturnDto()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var roleId = 1;
            var createUserRoleDto = new CreateUserRoleDto(userId, roleId);
            var userLogin = "testuser";
            var roleName = "TestRole";

            var user = User.CreateExisting(userId, userLogin, "hashed");
            var role = Role.CreateExisting(roleId, roleName);

            // Мокируем репозитории
            _userRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<User>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _roleRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Role>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(role);
            // Добавляем мок для GetListValuesAsync, который проверяет, что у пользователя нет роли
            _roleRepositoryMock.Setup(r => r.GetListValuesAsync(It.IsAny<BaseSpecification<Role>>(), It.IsAny<Expression<Func<Role, int>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<int> { 2, 3 }); // Роль с ID = 1 не найдена в списке

            _userRoleRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<UserRole>(), It.IsAny<CancellationToken>()));
            _cacheServiceMock.Setup(cs => cs.RemoveAsync(CacheKeys.User.Roles(userId), It.IsAny<CancellationToken>()));

            // Act
            var result = await _userRoleService.CreateAsync(createUserRoleDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Login.Should().Be(userLogin);
            result.Data.RoleName.Should().Be(roleName);

            _userRoleRepositoryMock.Verify(r => r.CreateAsync(It.Is<UserRole>(ur => ur.UserId == userId && ur.RoleId == roleId), It.IsAny<CancellationToken>()), Times.Once);
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _cacheServiceMock.Verify(cs => cs.RemoveAsync(CacheKeys.User.Roles(userId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_UserNotFound_ShouldReturnFailure()
        {
            // Arrange
            var createUserRoleDto = new CreateUserRoleDto(Guid.NewGuid(), 1);
            _userRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<User>>(), It.IsAny<CancellationToken>())).ReturnsAsync((User)null);

            // Act
            var result = await _userRoleService.CreateAsync(createUserRoleDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.User.NotFoundById(createUserRoleDto.UserId));
        }

        [Fact]
        public async Task CreateAsync_RoleNotFound_ShouldReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var createUserRoleDto = new CreateUserRoleDto(userId, 1);
            var user = User.CreateExisting(userId, "testuser", "hashed");

            _userRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<User>>(), It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _roleRepositoryMock.Setup(r => r.GetListValuesAsync(It.IsAny<BaseSpecification<Role>>(), It.IsAny<Expression<Func<Role, int>>>(), It.IsAny<CancellationToken>())).ReturnsAsync([2]);
            _roleRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Role>>(), It.IsAny<CancellationToken>())).ReturnsAsync((Role)null);

            // Act
            var result = await _userRoleService.CreateAsync(createUserRoleDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.Role.NotFoundById(createUserRoleDto.RoleId));
        }

        [Fact]
        public async Task CreateAsync_UserAlreadyHasRole_ShouldReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var roleId = 1;
            var createUserRoleDto = new CreateUserRoleDto(userId, roleId);
            var user = User.CreateExisting(userId, "testuser", "hashed");
            var role = Role.CreateExisting(roleId, "TestRole");

            _userRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<User>>(), It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _roleRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Role>>(), It.IsAny<CancellationToken>())).ReturnsAsync(role);
            // Имитируем, что у пользователя уже есть эта роль
            _roleRepositoryMock.Setup(r => r.GetListValuesAsync(It.IsAny<BaseSpecification<Role>>(), It.IsAny<Expression<Func<Role, int>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<int> { roleId, 2 });

            // Act
            var result = await _userRoleService.CreateAsync(createUserRoleDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.Role.UserAlreadyHasRole(roleId));
        }

        [Fact]
        public async Task DeleteAsync_ValidUserAndRole_ShouldRemoveAndReturnSuccess()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var roleId = 1;
            var userLogin = "testuser";
            var roleName = "TestRole";

            var user = User.CreateExisting(userId, userLogin, "hashed");
            var role = Role.CreateExisting(roleId, roleName);
            user.AddRole(role);

            // Мокируем, что у пользователя есть нужная роль
            var userRole = UserRole.Create(user.Id, role.Id); // UserRole сущность будет создана внутри

            _userRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<User>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _roleRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Role>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(role);
            // Важно: Мокаем UserRoleRepository, чтобы он вернул именно ту UserRole сущность, которую мы удаляем
            _userRoleRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<UserRole>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(userRole);
            _userRoleRepositoryMock.Setup(r => r.Remove(It.IsAny<UserRole>()));
            _cacheServiceMock.Setup(cs => cs.RemoveAsync(CacheKeys.User.Roles(userId), It.IsAny<CancellationToken>()));

            // Act
            var result = await _userRoleService.DeleteAsync(userId, roleId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            // Проверяем, что UserRole был удален из коллекции пользователя (если это важно для теста)
            // user.Roles.Should().NotContain(r => r.Id == roleId); // Для этого нужен доступ к User.Roles как ICollection

            _userRoleRepositoryMock.Verify(r => r.Remove(It.Is<UserRole>(ur => ur.UserId == userId && ur.RoleId == roleId)), Times.Once);
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _cacheServiceMock.Verify(cs => cs.RemoveAsync(CacheKeys.User.Roles(userId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_UserNotFound_ShouldReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _userRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<User>>(), It.IsAny<CancellationToken>())).ReturnsAsync((User)null);

            // Act
            var result = await _userRoleService.DeleteAsync(userId, 1);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.User.NotFoundById(userId));
        }

        [Fact]
        public async Task DeleteAsync_RoleNotFoundForUser_ShouldReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var roleId = 1;
            var user = User.CreateExisting(userId, "testuser", "hashed");

            _userRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<User>>(), It.IsAny<CancellationToken>())).ReturnsAsync(user);

            // Act
            var result = await _userRoleService.DeleteAsync(userId, roleId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.Role.NotFoundById(roleId)); // Здесь ошибка в DomainError, должно быть что-то вроде RoleNotAssignedToUser
        }

        [Fact]
        public async Task UpdateAsync_ValidChange_ShouldUpdateRoleAndReturnSuccess()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var fromRoleId = 1;
            var toRoleId = 2;
            var userLogin = "testuser";
            var fromRoleName = "OldRole";
            var toRoleName = "NewRole";

            var user = User.CreateExisting(userId, userLogin, "hashed");
            var fromRole = Role.CreateExisting(fromRoleId, fromRoleName);
            var toRole = Role.CreateExisting(toRoleId, toRoleName);
            var userRole = UserRole.Create(userId, toRoleId);

            // User.AddRole(fromRole) создает UserRole сущность
            user.AddRole(fromRole); // Добавляем старую роль

            var updateUserRoleDto = new UpdateUserRoleDto(fromRoleId, toRoleId);

            // Мокируем репозитории
            _userRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<User>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _roleRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Role>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(toRole);
            // Мокируем получение UserRole для удаления
            _userRoleRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<UserRole>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(UserRole.Create(userId, toRoleId)); // Возвращаем UserRole сущность
            _userRoleRepositoryMock.Setup(r => r.Remove(It.IsAny<UserRole>()));
            _userRoleRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<UserRole>(), It.IsAny<CancellationToken>()));
            _cacheServiceMock.Setup(cs => cs.RemoveAsync(CacheKeys.User.Roles(userId), It.IsAny<CancellationToken>()));

            // Act
            var result = await _userRoleService.UpdateAsync(userId, updateUserRoleDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Login.Should().Be(userLogin);
            result.Data.RoleName.Should().Be(toRoleName);

            _userRoleRepositoryMock.Verify(r => r.Remove(It.IsAny<UserRole>()), Times.Once);
            _userRoleRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<UserRole>()), Times.Once);
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _cacheServiceMock.Verify(cs => cs.RemoveAsync(CacheKeys.User.Roles(userId), It.IsAny<CancellationToken>()), Times.Once);
            _transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_UserNotFound_ShouldReturnFailure()
        {
            // Arrange
            _userRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<User>>(), It.IsAny<CancellationToken>())).ReturnsAsync((User)null);
            var userId = Guid.NewGuid();

            // Act
            var result = await _userRoleService.UpdateAsync(userId, new UpdateUserRoleDto(1, 2));

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.User.NotFoundById(userId)); // Guid.Empty так как он не был передан
        }

        [Fact]
        public async Task UpdateAsync_RoleNotFound_ShouldReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = User.CreateExisting(userId, "testuser", "hashed");
            _userRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<User>>(), It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _roleRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Role>>(), It.IsAny<CancellationToken>())).ReturnsAsync((Role)null); // Новая роль не найдена

            // Act
            var result = await _userRoleService.UpdateAsync(userId, new UpdateUserRoleDto(1, 2));

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.Role.UserRoleNotFound(1));
        }

        [Fact]
        public async Task UpdateAsync_UserAlreadyHasTargetRole_ShouldReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var fromRoleId = 1;
            var toRoleId = 2;
            var user = User.CreateExisting(userId, "testuser", "hashed");
            var fromRole = Role.CreateExisting(fromRoleId, "OldRole");
            var toRole = Role.CreateExisting(toRoleId, "NewRole");

            user.AddRole(fromRole); // Добавляем старую роль
            user.AddRole(toRole); // Добавляем новую роль (чтобы имитировать, что она уже есть)

            _userRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<User>>(), It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _roleRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Role>>(), It.IsAny<CancellationToken>())).ReturnsAsync(toRole); // Новая роль найдена

            // Act
            var result = await _userRoleService.UpdateAsync(userId, new UpdateUserRoleDto(fromRoleId, toRoleId));

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.Role.UserAlreadyHasRole(toRoleId));
        }

        [Fact]
        public async Task UpdateAsync_TransactionRollbackOnException_ShouldRollback()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = User.CreateExisting(userId, "testuser", "hashed");
            var fromRole = Role.CreateExisting(1, "OldRole");
            var toRole = Role.CreateExisting(2, "NewRole");
            user.AddRole(fromRole);

            _userRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<User>>(), It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _roleRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Role>>(), It.IsAny<CancellationToken>())).ReturnsAsync(toRole);
            _userRoleRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<UserRole>>(), It.IsAny<CancellationToken>())).ReturnsAsync(UserRole.Create(user.Id, toRole.Id));
            _userRoleRepositoryMock.Setup(r => r.Remove(It.IsAny<UserRole>()));
            _userRoleRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<UserRole>(), It.IsAny<CancellationToken>()));
            // Имитируем исключение при SaveChangesAsync
            _uowMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Simulated error"));

            // Act
            var result = await _userRoleService.UpdateAsync(userId, new UpdateUserRoleDto(1, 2));

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.General.InternalServerError()); // Ожидаем общую ошибку сервера
            _transactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByUserIdAsync_UserHasRoles_ShouldReturnRoleNames()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var roleNames = new List<string> { "Admin", "User" };

            _cacheServiceMock.Setup(cs => cs.GetOrCreateAsync(CacheKeys.User.Roles(userId), It.IsAny<Func<CancellationToken, Task<List<string>>>>()))
                .ReturnsAsync(roleNames);

            // Act
            var result = await _userRoleService.GetByUserIdAsync(userId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().HaveCount(2);
            result.Data.Should().BeEquivalentTo(roleNames);
            _cacheServiceMock.Verify(cs => cs.GetOrCreateAsync(CacheKeys.User.Roles(userId), It.IsAny<Func<CancellationToken, Task<List<string>>>>()), Times.Once);
        }

        [Fact]
        public async Task GetByUserIdAsync_UserHasNoRoles_ShouldReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var emptyRoleNames = new List<string>();

            _cacheServiceMock.Setup(cs => cs.GetOrCreateAsync(CacheKeys.User.Roles(userId), It.IsAny<Func<CancellationToken, Task<List<string>>>>()))
                .ReturnsAsync(emptyRoleNames);

            // Act
            var result = await _userRoleService.GetByUserIdAsync(userId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.Role.NotFoundByUser(userId));
        }
    }
}
