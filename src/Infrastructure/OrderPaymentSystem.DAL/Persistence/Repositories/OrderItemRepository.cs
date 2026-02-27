using OrderPaymentSystem.DAL.Persistence.Repositories.Base;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.DAL.Persistence.Repositories;

/// <summary>
/// Репозиторий для работы с сущностью <see cref="OrderItem"/>
/// </summary>
internal class OrderItemRepository : BaseRepository<OrderItem>, IOrderItemRepository
{
    /// <summary>
    /// Конструктор репозитория
    /// </summary>
    /// <param name="dbContext">Контекст для работы с БД</param>
    public OrderItemRepository(ApplicationDbContext dbContext) : base(dbContext) { }
}
