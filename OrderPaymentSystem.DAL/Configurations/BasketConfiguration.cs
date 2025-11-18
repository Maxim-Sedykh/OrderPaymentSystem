using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.DAL.Configurations;

public class BasketConfiguration : IEntityTypeConfiguration<Basket>
{
    public void Configure(EntityTypeBuilder<Basket> builder)
    {
        builder.HasData(new Basket
        {
            Id = 1,
            UserId = new Guid("0f8fad5b-d9cb-469f-a165-70867728950e"),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 1,
        },
        new Basket
        {
            Id = 2,
            UserId = new Guid("7c9e6679-7425-40de-944b-e07fc1f90ae7"),
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
