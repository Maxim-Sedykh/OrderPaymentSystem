using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Interfaces.Repositories.Base;

namespace OrderPaymentSystem.Domain.Interfaces.Repositories;

/// <summary>
/// Интерфейс репозитория для работы с сущностью <see cref="BasketItem"/>
/// </summary>
public interface IBasketItemRepository : IBaseRepository<BasketItem>;
