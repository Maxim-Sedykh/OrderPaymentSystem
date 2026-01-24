using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.DAL.Persistence.Configurations;

internal class BasketItemConfiguration : IEntityTypeConfiguration<BasketItem>
{
	public void Configure(EntityTypeBuilder<BasketItem> builder)
	{
		builder.HasKey(bi => bi.Id);
        builder.HasIndex(bi => bi.UserId);

        builder.Property(bi => bi.Quantity)
			.IsRequired();

		builder.Property(bi => bi.CreatedAt)
			.IsRequired();

		builder.HasOne(bi => bi.User)
			.WithMany(u => u.BasketItems)
			.HasForeignKey(bi => bi.UserId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(bi => bi.Product)
			.WithMany(p => p.BasketItems)
			.HasForeignKey(bi => bi.ProductId)
			.OnDelete(DeleteBehavior.Restrict);
	}
}
