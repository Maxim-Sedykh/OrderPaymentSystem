using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.DAL.Interceptors;
using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.DAL.Persistence;

/// <summary>
/// Контекст для работы с базой данной приложения
/// </summary>
public class ApplicationDbContext : DbContext
{
    /// <summary>
    /// Конструктор контекста.
    /// </summary>
    /// <param name="options">Настройки работы контекста</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    /// <summary>
    /// Элементы корзины
    /// </summary>
    public DbSet<BasketItem> BasketItems { get; init; }

    /// <summary>
    /// Заказы
    /// </summary>
    public DbSet<Order> Orders { get; init; }

    /// <summary>
    /// Элементы заказа
    /// </summary>
    public DbSet<OrderItem> OrderItems { get; init; }

    /// <summary>
    /// Платежи
    /// </summary>
    public DbSet<Payment> Payments { get; init; }

    /// <summary>
    /// Товары
    /// </summary>
    public DbSet<Product> Products { get; init; }

    /// <summary>
    /// Роли
    /// </summary>
    public DbSet<Role> Roles { get; init; }

    /// <summary>
    /// Refresh-токены пользователей
    /// </summary>
    public DbSet<UserToken> UserTokens { get; init; }

    /// <summary>
    /// Пользователи
    /// </summary>
    public DbSet<User> Users { get; init; }
    
    /// <summary>
    /// Роли пользователей
    /// </summary>
    public DbSet<UserRole> UserRoles { get; init; }

    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(new AuditInterceptor());
    }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
