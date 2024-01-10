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
    public class OrderConfigurations : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.HasOne(x => x.Payment)
                    .WithOne(x => x.Order)
                    .HasPrincipalKey<Order>(x => x.Id)
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
