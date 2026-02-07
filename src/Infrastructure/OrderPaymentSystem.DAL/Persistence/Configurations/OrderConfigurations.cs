using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderPaymentSystem.DAL.Persistence.Constants;
using OrderPaymentSystem.Domain.Entities;
using static OrderPaymentSystem.Domain.Constants.ValidationConstants.Address;

namespace OrderPaymentSystem.DAL.Persistence.Configurations;

internal class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.TotalAmount)
            .IsRequired()
            .HasColumnType(DbColumnFormats.Decimal18_2);

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Payment)
               .WithOne(p => p.Order)
               .HasForeignKey<Payment>(p => p.OrderId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);

        builder.OwnsOne(c => c.DeliveryAddress, a =>
        {
            a.Property(x => x.Street).IsRequired().HasMaxLength(MaxStreetLength);
            a.Property(x => x.City).IsRequired().HasMaxLength(MaxCityLength);
            a.Property(x => x.ZipCode).IsRequired().HasMaxLength(MaxZipCodeLength);
        });

        builder.Navigation(o => o.DeliveryAddress).IsRequired(false);

        builder.HasMany(o => o.Items)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(o => o.CreatedAt).IsRequired();
        builder.Property(o => o.UpdatedAt).IsRequired(false);
    }
}