using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.DAL.Interceptors;
using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.DAL.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<BasketItem> BasketItems { get; init; }
    public DbSet<Order> Orders { get; init; }
    public DbSet<OrderItem> OrderItems { get; init; }
    public DbSet<Payment> Payments { get; init; }
    public DbSet<Product> Products { get; init; }
    public DbSet<Role> Roles { get; init; }
    public DbSet<UserToken> UserTokens { get; init; }
    public DbSet<User> Users { get; init; }
    public DbSet<UserRole> UserRoles { get; init; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(new AuditInterceptor());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
