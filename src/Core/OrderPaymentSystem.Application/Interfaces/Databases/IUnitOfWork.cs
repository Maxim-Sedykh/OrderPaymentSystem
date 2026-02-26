using Microsoft.EntityFrameworkCore.Storage;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories;

namespace OrderPaymentSystem.Application.Interfaces.Databases;

/// <summary>
/// Интерфейс для реализации паттерна Unit of work, для работы с транзакциями EF Core
/// </summary>
public interface IUnitOfWork
{
    IOrderRepository Orders { get; }
    IProductRepository Products { get; }
    IOrderItemRepository OrderItems { get; }
    IBasketItemRepository BasketItems { get; }
    IPaymentRepository Payments { get; }
    IRoleRepository Roles { get; }
    IUserRepository Users { get; }
    IUserRoleRepository UserRoles { get; }
    IUserTokenRepository UserTokens { get; }

    /// <summary>
    /// Сохранить состояние сущности в БД
    /// </summary>
    /// <param name="ct">Токен для отмены операции</param>
    /// <returns>Количество обработанных кортежей в таблицах</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Начинает новую транзакцию
    /// </summary>
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
