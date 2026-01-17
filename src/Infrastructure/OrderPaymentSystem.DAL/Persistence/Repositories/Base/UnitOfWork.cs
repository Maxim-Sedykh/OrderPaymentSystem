using Microsoft.EntityFrameworkCore.Storage;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Domain.Interfaces.Repositories;

namespace OrderPaymentSystem.DAL.Persistence.Repositories.Base;

/// <summary>
/// Unit of work. Сервис для работы с транзакциями EF Core
/// </summary>
/// <param name="context"></param>
public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    public IOrderRepository Orders { get; }
    public IProductRepository Products { get; }
    public IOrderItemRepository OrderItems { get; }
    public IBasketItemRepository BasketItems { get; }
    public IPaymentRepository Payments { get; }
    public IRoleRepository Roles { get; }
    public IUserRepository Users { get; }
    public IUserRoleRepository UserRoles { get; }
    public IUserTokenRepository UserToken { get; }

    /// <inheritdoc/>
    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return await context.Database.BeginTransactionAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }
}
