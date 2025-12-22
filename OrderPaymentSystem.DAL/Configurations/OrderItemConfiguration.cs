using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderPaymentSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderPaymentSystem.DAL.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(oi => oi.Id);

            builder.Property(oi => oi.Quantity)
                .IsRequired();

            builder.Property(oi => oi.UnitPrice)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(oi => oi.ItemTotalSum)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            // Связь один-ко-многим с Order (уже сконфигурирована в OrderConfiguration)
            builder.HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade); // Если Order удаляется, OrderItem также удаляется

            // Связь один-ко-многим с Product
            builder.HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict); // Продукт не удаляется, если он является частью заказа
        }
    }
}
