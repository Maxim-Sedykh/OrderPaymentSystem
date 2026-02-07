using OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories.Base;
using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories;

/// <summary>
/// Интерфейс репозитория для работы с сущностью <see cref="BasketItem"/>
/// </summary>
public interface IBasketItemRepository : IBaseRepository<BasketItem>;
