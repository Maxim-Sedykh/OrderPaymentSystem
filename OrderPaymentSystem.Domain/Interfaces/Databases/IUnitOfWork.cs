using Microsoft.EntityFrameworkCore.Storage;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Interfaces.Repositories;

namespace OrderPaymentSystem.Domain.Interfaces.Databases;

public interface IUnitOfWork: IStateSaveChanges
{
    Task<IDbContextTransaction> BeginTransactionAsync();

    IBaseRepository<User> Users { get; set; }

    IBaseRepository<Role> Roles { get; set; }

    IBaseRepository<UserRole> UserRoles { get; set; }

    IBaseRepository<Basket> Baskets { get; set; }

    IBaseRepository<Payment> Payments { get; set; }

    IBaseRepository<Order> Orders { get; set; }
}
