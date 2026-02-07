using OrderPaymentSystem.DAL.Persistence.Repositories.Base;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.DAL.Persistence.Repositories;

internal class OrderRepository : BaseRepository<Order>, IOrderRepository
{
    public OrderRepository(ApplicationDbContext dbContext) : base(dbContext) { }
}
