using MapsterMapper;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using OrderPaymentSystem.Application.Interfaces.Databases;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace OrderPaymentSystem.UnitTests.Configurations
{
    public abstract class ServiceTestsBase
    {
        protected readonly Mock<IUnitOfWork> UowMock = new();
        protected readonly Mock<IMapper> MapperMock = new();
        protected readonly Mock<IDbContextTransaction> TransactionMock = new();

        protected ServiceTestsBase()
        {
            UowMock.Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                   .ReturnsAsync(TransactionMock.Object);
            UowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                   .ReturnsAsync(1);
        }

        protected void SetupRepository<TRepo>(Expression<Func<IUnitOfWork, TRepo>> expression, TRepo repo) where TRepo : class
        {
            UowMock.Setup(expression).Returns(repo);
        }
    }
}
