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
    public class ProductConfigurations : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.ProductName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Description).IsRequired().HasMaxLength(2000);

            builder.HasMany<Warehouse>(x => x.Warehouses)
                .WithOne(x => x.Product)
                .HasForeignKey(x => x.ProductId)
                .HasPrincipalKey(x => x.Id);

            builder.HasMany<Order>(x => x.Orders)
                .WithOne(x => x.Product)
                .HasForeignKey(x => x.ProductId)
                .HasPrincipalKey(x => x.Id);
        }
    }
}
