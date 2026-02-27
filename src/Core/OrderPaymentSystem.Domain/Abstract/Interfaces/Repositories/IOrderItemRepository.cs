using OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories.Base;
using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories;

/// <summary>
/// Интерфейс репозитория для работы с <see cref="OrderItem"/>
/// </summary>
public interface IOrderItemRepository : IBaseRepository<OrderItem>;
