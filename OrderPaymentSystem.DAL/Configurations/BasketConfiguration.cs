using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.Domain.Entity;

namespace OrderPaymentSystem.DAL.Configurations
{
    public class BasketConfiguration : IEntityTypeConfiguration<Basket>
    {
        public void Configure(EntityTypeBuilder<Basket> builder)
        {
            builder.HasData(new Basket
            {
                Id = 1,
                UserId = 1,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
            },
            new Basket
            {
                Id = 2,
                UserId = 2,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
            });

            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.HasMany(x => x.Payments)
                    .WithOne(x => x.Basket)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Orders)
                    .WithOne(x => x.Basket)
                    .HasForeignKey(x => x.BasketId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired(false);
        }
    }
}
