using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.DAL.Configurations;

public class BasketItemConfiguration : IEntityTypeConfiguration<BasketItem>
{
	public void Configure(EntityTypeBuilder<BasketItem> builder)
	{
		builder.HasKey(bi => bi.Id);

		builder.Property(bi => bi.Quantity)
			.IsRequired();

		builder.Property(bi => bi.CreatedAt)
			.IsRequired();

		// Связь один-ко-многим с User
		builder.HasOne(bi => bi.User)
			.WithMany(u => u.BasketItems)
			.HasForeignKey(bi => bi.UserId)
			.OnDelete(DeleteBehavior.Cascade); // Если User удаляется, BasketItems также удаляются

		// Связь один-ко-многим с Product
		builder.HasOne(bi => bi.Product)
			.WithMany(p => p.BasketItems)
			.HasForeignKey(bi => bi.ProductId)
			.OnDelete(DeleteBehavior.Restrict); // Продукт не удаляется, если он находится в корзине
	}
}
