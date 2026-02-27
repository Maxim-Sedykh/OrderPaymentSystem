using OrderPaymentSystem.DAL.Persistence.Repositories.Base;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.DAL.Persistence.Repositories;

/// <summary>
/// Репозиторий для работы с сущностью <see cref="Payment"/>
/// </summary>
internal class PaymentRepository : BaseRepository<Payment>, IPaymentRepository
{
    /// <summary>
    /// Конструктор репозитория
    /// </summary>
    /// <param name="dbContext">Контекст для работы с БД</param>
    public PaymentRepository(ApplicationDbContext dbContext) : base(dbContext) { }
}
