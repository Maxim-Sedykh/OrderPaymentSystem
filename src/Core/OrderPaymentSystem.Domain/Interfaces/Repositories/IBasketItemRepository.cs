using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Interfaces.Repositories.Base;

namespace OrderPaymentSystem.Domain.Interfaces.Repositories;

/// <summary>
/// Интерфейс репозитория для работы с сущностью <see cref="BasketItem"/>
/// </summary>
public interface IBasketItemRepository : IBaseRepository<BasketItem>
{
    /// <summary>
    /// Получить элемент корзины по Id
    /// </summary>
    /// <param name="id">Id элемента корзины</param>
    /// <returns>Сущность элемента корзины</returns>
    Task<BasketItem> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    IQueryable<BasketItem> GetByUserIdQuery(Guid userId);

    Task<BasketItem> GetByIdWithProductAsync(long id, CancellationToken cancellationToken = default);
}
