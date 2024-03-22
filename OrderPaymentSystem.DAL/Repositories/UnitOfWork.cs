using Microsoft.EntityFrameworkCore.Storage;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Interfaces.Databases;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IBaseRepository<User> Users { get; set; }
        public IBaseRepository<Role> Roles { get; set; }
        public IBaseRepository<Basket> Baskets { get; set; }
        public IBaseRepository<UserRole> UserRoles { get; set; }

        public UnitOfWork(ApplicationDbContext context, IBaseRepository<User> users, IBaseRepository<Role> roles,
            IBaseRepository<UserRole> userRoles, IBaseRepository<Basket> baskets)
        {
            _context = context;
            Users = users;
            Roles = roles;
            UserRoles = userRoles;
            Baskets = baskets;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
