using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderPaymentSystem.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.DAL.Configurations
{
    public class PaymentConfigurations : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.OwnsOne(c => c.DeliveryAddress, a =>
            {
                a.Property(x => x.Street).IsRequired().HasMaxLength(50);
                a.Property(x => x.City).IsRequired().HasMaxLength(50);
                a.Property(x => x.ZipCode).IsRequired().HasMaxLength(50);
            });

            builder.HasMany(x => x.Orders)
                .WithOne(x => x.Payment)
                .HasForeignKey(x => x.PaymentId)
                .IsRequired(false);
        }
    }
}
