using OrderPaymentSystem.DAL.Persistence.Repositories.Base;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.DAL.Persistence.Repositories;

internal class OrderItemRepository : BaseRepository<OrderItem>, IOrderItemRepository
{
    public OrderItemRepository(ApplicationDbContext dbContext) : base(dbContext) { }
}
