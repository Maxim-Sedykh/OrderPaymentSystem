using Microsoft.EntityFrameworkCore.Storage;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories;

namespace OrderPaymentSystem.Application.Interfaces.Databases;

/// <summary>
/// Интерфейс для реализации паттерна Unit of work, для работы с транзакциями EF Core
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Репозиторий для работы с заказами
    /// </summary>
    IOrderRepository Orders { get; }

    /// <summary>
    /// Репозиторий для работы с товарами
    /// </summary>
    IProductRepository Products { get; }

    /// <summary>
    /// Репозиторий для работы с элементами заказов
    /// </summary>
    IOrderItemRepository OrderItems { get; }

    /// <summary>
    /// Репозиторий для работы с элементами корзины
    /// </summary>
    IBasketItemRepository BasketItems { get; }

    /// <summary>
    /// Репозиторий для работы с платежами
    /// </summary>
    IPaymentRepository Payments { get; }

    /// <summary>
    /// Репозиторий для работы с ролями
    /// </summary>
    IRoleRepository Roles { get; }

    /// <summary>
    /// Репозиторий для работы с пользователями
    /// </summary>
    IUserRepository Users { get; }

    /// <summary>
    /// Репозиторий для работы с ролями пользователя
    /// </summary>
    IUserRoleRepository UserRoles { get; }

    /// <summary>
    /// Репозиторий для работы с токенами пользователя
    /// </summary>
    IUserTokenRepository UserTokens { get; }

    /// <summary>
    /// Сохранить состояние сущности в БД
    /// </summary>
    /// <param name="cancellationToken">Токен для отмены операции</param>
    /// <returns>Количество обработанных кортежей в таблицах</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Начинает новую транзакцию
    /// </summary>
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
