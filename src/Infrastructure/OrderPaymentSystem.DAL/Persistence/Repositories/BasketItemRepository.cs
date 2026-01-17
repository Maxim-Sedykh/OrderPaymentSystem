using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.DAL.Persistence.Repositories.Base;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Interfaces.Repositories;

namespace OrderPaymentSystem.DAL.Persistence.Repositories;

internal class BasketItemRepository : BaseRepository<BasketItem>, IBasketItemRepository
{
    internal BasketItemRepository(ApplicationDbContext dbContext) : base(dbContext) { }
}
