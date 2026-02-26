using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderPaymentSystem.DAL.Persistence.Constants;
using OrderPaymentSystem.Domain.Entities;
using static OrderPaymentSystem.Domain.Constants.ValidationConstants.Product;

namespace OrderPaymentSystem.DAL.Persistence.Configurations;

internal class ProductConfiguration : IEntityTypeConfiguration<Product>
{
	public void Configure(EntityTypeBuilder<Product> builder)
	{
        builder.HasKey(p => p.Id);
        builder.HasIndex(p => p.Name);

        builder
            .Property(p => p.RowVersion)
            .IsRowVersion()
            .IsRequired()
            .ValueGeneratedOnAddOrUpdate();

        builder.Property(p => p.Name)
			.IsRequired()
			.HasMaxLength(MaxNameLength);

        builder.Property(p => p.Description)
			.HasMaxLength(MaxDescriptionLength)
			.IsRequired(false);

		builder.Property(p => p.Price)
			.IsRequired()
			.HasColumnType(DbColumnFormats.Decimal18_2);

		builder.HasMany(p => p.OrderItems)
			.WithOne(oi => oi.Product)
			.HasForeignKey(oi => oi.ProductId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.HasMany(p => p.BasketItems)
			.WithOne(bi => bi.Product)
			.HasForeignKey(bi => bi.ProductId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.Property(p => p.CreatedAt).IsRequired();
		builder.Property(p => p.UpdatedAt).IsRequired(false);
	}
}
