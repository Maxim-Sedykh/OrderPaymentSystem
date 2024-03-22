using Microsoft.EntityFrameworkCore.Storage;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Domain.Interfaces.Databases
{
    public interface IUnitOfWork: IStateSaveChanges
    {
        Task<IDbContextTransaction> BeginTransactionAsync();

        IBaseRepository<User> Users { get; set; }

        IBaseRepository<Role> Roles { get; set; }

        IBaseRepository<UserRole> UserRoles { get; set; }

        IBaseRepository<Basket> Baskets { get; set; }
    }
}
