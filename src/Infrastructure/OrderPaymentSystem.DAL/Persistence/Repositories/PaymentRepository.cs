using OrderPaymentSystem.DAL.Persistence.Repositories.Base;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Interfaces.Repositories;

namespace OrderPaymentSystem.DAL.Persistence.Repositories;

internal class PaymentRepository : BaseRepository<Payment>, IPaymentRepository
{
    public PaymentRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public Task<bool> ExistsByOrderIdAsync(long orderId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Payment> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public IQueryable<Payment> GetByIdQuery(long id)
    {
        throw new NotImplementedException();
    }

    public IQueryable<Payment> GetByOrderIdQuery(long orderId)
    {
        throw new NotImplementedException();
    }
}
