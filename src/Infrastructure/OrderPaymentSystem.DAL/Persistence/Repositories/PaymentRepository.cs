using OrderPaymentSystem.DAL.Persistence.Repositories.Base;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Interfaces.Repositories;

namespace OrderPaymentSystem.DAL.Persistence.Repositories;

internal class PaymentRepository : BaseRepository<Payment>, IPaymentRepository
{
    public PaymentRepository(ApplicationDbContext dbContext) : base(dbContext) { }
}
