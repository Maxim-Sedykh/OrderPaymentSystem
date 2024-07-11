using Microsoft.EntityFrameworkCore.Storage;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Interfaces.Databases;
using OrderPaymentSystem.Domain.Interfaces.Repositories;

namespace OrderPaymentSystem.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IBaseRepository<User> Users { get; set; }
        public IBaseRepository<Role> Roles { get; set; }
        public IBaseRepository<Basket> Baskets { get; set; }
        public IBaseRepository<UserRole> UserRoles { get; set; }
        public IBaseRepository<Payment> Payments { get; set; }
        public IBaseRepository<Order> Orders { get; set; }

        public UnitOfWork(ApplicationDbContext context, IBaseRepository<User> users, IBaseRepository<Role> roles,
            IBaseRepository<UserRole> userRoles, IBaseRepository<Basket> baskets, IBaseRepository<Payment> payments,
            IBaseRepository<Order> orders)
        {
            _context = context;
            Users = users;
            Roles = roles;
            UserRoles = userRoles;
            Baskets = baskets;
            Payments = payments;
            Orders = orders;
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
