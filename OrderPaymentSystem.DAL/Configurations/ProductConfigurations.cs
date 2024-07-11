using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderPaymentSystem.Domain.Entity;

namespace OrderPaymentSystem.DAL.Configurations
{
    public class ProductConfigurations : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasData(new Product
            {
                Id = 1,
                ProductName = "Алмазная мозаика",
                Description = "Очень красивая мозаика",
                Cost = 1500,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1
            },
            new Product
            {
                Id = 2,
                ProductName = "Ночник",
                Description = "Красивый ночник в виде панды",
                Cost = 600,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1
            });

            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.ProductName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Description).IsRequired().HasMaxLength(2000);

            builder.HasMany(x => x.Orders)
                .WithOne(x => x.Product)
                .HasForeignKey(x => x.ProductId)
                .HasPrincipalKey(x => x.Id);
        }
    }
}
