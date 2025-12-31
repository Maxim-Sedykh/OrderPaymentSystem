using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.DAL.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
	public void Configure(EntityTypeBuilder<Product> builder)
	{
        builder
		 .Property(p => p.RowVersion)
		.IsRowVersion();

        builder.HasKey(p => p.Id);

		builder.Property(p => p.Name)
			.IsRequired()
			.HasMaxLength(100);

		builder.Property(p => p.Description)
			.HasMaxLength(1000)
			.IsRequired(false); // Описание может быть опциональным

		builder.Property(p => p.Price)
			.IsRequired()
			.HasColumnType("decimal(18,2)"); // Важно для decimal, чтобы указать точность и масштаб

		// Связь один-ко-многим с OrderItem
		builder.HasMany(p => p.OrderItems)
			.WithOne(oi => oi.Product)
			.HasForeignKey(oi => oi.ProductId)
			.OnDelete(DeleteBehavior.Restrict); // Продукт не должен удаляться, если он является частью заказа

		// Связь один-ко-многим с BasketItem
		builder.HasMany(p => p.BasketItems)
			.WithOne(bi => bi.Product)
			.HasForeignKey(bi => bi.ProductId)
			.OnDelete(DeleteBehavior.Restrict); // Продукт не должен удаляться, если он находится в корзине

		// Аудит-поля
		builder.Property(p => p.CreatedAt).IsRequired();
		builder.Property(p => p.CreatedBy).IsRequired();
		builder.Property(p => p.UpdatedAt).IsRequired(false);
		builder.Property(p => p.UpdatedBy).IsRequired(false);
	}
}
