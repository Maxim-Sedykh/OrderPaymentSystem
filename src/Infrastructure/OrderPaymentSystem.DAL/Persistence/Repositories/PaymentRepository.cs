using OrderPaymentSystem.DAL.Persistence.Repositories.Base;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.DAL.Persistence.Repositories;

internal class PaymentRepository : BaseRepository<Payment>, IPaymentRepository
{
    public PaymentRepository(ApplicationDbContext dbContext) : base(dbContext) { }
}
