using OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories.Base;
using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories;

/// <summary>
/// Интерфейс репозитория для работы с <see cref="User"/>
/// </summary>
public interface IUserRepository : IBaseRepository<User>;