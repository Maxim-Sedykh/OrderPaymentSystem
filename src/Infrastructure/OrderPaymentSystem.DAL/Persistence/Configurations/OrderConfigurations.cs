using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.DAL.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.TotalAmount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<string>(); // Храним enum как строку в БД для читаемости

        // Связь один-ко-многим с User (уже сконфигурирована в UserConfiguration)
        builder.HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict); // Заказы не удаляются при удалении пользователя

        // Связь один-к-одному с Payment (PaymentId в Order является nullable)
        // Order.PaymentId является внешним ключом к Payment.Id, но это обратная связь,
        // так как Payment имеет OrderId. Лучше настраивать с Payment стороны.
        // Если PaymentId в Order является внешним ключом, то это 1:0..1.
        // PaymentId у Order nullable, поэтому:
        builder.HasOne(o => o.Payment)
               .WithOne(p => p.Order)
               .HasForeignKey<Payment>(p => p.OrderId) // FK находится в Payment, а не в Order
               .IsRequired(false) // PaymentId в Order может быть null
               .OnDelete(DeleteBehavior.Restrict); // Не удаляем Payment при удалении Order, чтобы сохранить историю платежей.

        // Конфигурация Value Object Address как Owned Entity
        builder.OwnsOne(c => c.DeliveryAddress, a =>
        {
            a.Property(x => x.Street).IsRequired().HasMaxLength(50);
            a.Property(x => x.City).IsRequired().HasMaxLength(50);
            a.Property(x => x.ZipCode).IsRequired().HasMaxLength(50);
        });
        // Если DeliveryAddress может быть null
        builder.Navigation(o => o.DeliveryAddress).IsRequired(false);


        // Связь один-ко-многим с OrderItem
        builder.HasMany(o => o.Items)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade); // Позиции заказа удаляются при удалении заказа

        // Аудит-поля
        builder.Property(o => o.CreatedAt).IsRequired();
        builder.Property(o => o.CreatedBy).IsRequired();
        builder.Property(o => o.UpdatedAt).IsRequired(false);
        builder.Property(o => o.UpdatedBy).IsRequired(false);
    }
}