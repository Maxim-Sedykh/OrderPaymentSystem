using OrderPaymentSystem.DAL.Persistence.Repositories.Base;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.DAL.Persistence.Repositories;

/// <summary>
/// Репозиторий для работы с сущностью <see cref="UserRole"/>
/// </summary>
internal class UserRoleRepository : BaseRepository<UserRole>, IUserRoleRepository
{
    /// <summary>
    /// Конструктор репозитория
    /// </summary>
    /// <param name="dbContext">Контекст для работы с БД</param>
    public UserRoleRepository(ApplicationDbContext dbContext) : base(dbContext) { }
}
