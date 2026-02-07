using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using OrderPaymentSystem.Application.Interfaces.Auth;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Application.Interfaces.Services;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace OrderPaymentSystem.UnitTests.Configurations
{
    public static class AuthServiceMockConfigurations
    {
        public static Mock<IUnitOfWork> SetupBasicUnitOfWork()
        {
            var mock = new Mock<IUnitOfWork>();
            mock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            mock.Setup(uow => uow.BeginTransactionAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Mock.Of<IDbContextTransaction>());
            // Мокируем репозитории, которые сервис будет использовать
            mock.Setup(uow => uow.Users).Returns(Mock.Of<IUserRepository>());
            mock.Setup(uow => uow.UserToken).Returns(Mock.Of<IUserTokenRepository>());
            mock.Setup(uow => uow.Roles).Returns(Mock.Of<IRoleRepository>());
            mock.Setup(uow => uow.UserRoles).Returns(Mock.Of<IUserRoleRepository>());
            return mock;
        }

        public static Mock<IPasswordHasher> SetupPasswordHasher(bool verifyResult = true)
        {
            var mock = new Mock<IPasswordHasher>();
            mock.Setup(ph => ph.Hash(It.IsAny<string>())).Returns("hashed_password");
            mock.Setup(ph => ph.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(verifyResult);
            return mock;
        }

        public static Mock<IUserTokenService> SetupUserTokenService(string accessToken, string refreshToken)
        {
            var mock = new Mock<IUserTokenService>();
            mock.Setup(uts => uts.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>())).Returns(accessToken);
            mock.Setup(uts => uts.GenerateRefreshToken()).Returns(refreshToken);
            mock.Setup(uts => uts.GetClaimsFromUser(It.IsAny<User>())).Returns(new List<Claim>()); // Простая заглушка
            return mock;
        }
    }
}
