using OrderPaymentSystem.DAL.Persistence.Repositories.Base;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.DAL.Persistence.Repositories;

internal class BasketItemRepository : BaseRepository<BasketItem>, IBasketItemRepository
{
    public BasketItemRepository(ApplicationDbContext dbContext) : base(dbContext) { }
}
