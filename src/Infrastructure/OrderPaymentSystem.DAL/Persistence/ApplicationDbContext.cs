using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.DAL.Interceptors;
using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.DAL.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<BasketItem> BasketItems { get; }
    public DbSet<Order> Orders { get; }
    public DbSet<OrderItem> OrderItems { get; }
    public DbSet<Payment> Payments { get; }
    public DbSet<Product> Products { get; }
    public DbSet<Role> Roles { get; }
    public DbSet<UserToken> UserTokens { get; }
    public DbSet<User> Users { get; }
    public DbSet<UserRole> UserRoles { get; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(new AuditInterceptor());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
