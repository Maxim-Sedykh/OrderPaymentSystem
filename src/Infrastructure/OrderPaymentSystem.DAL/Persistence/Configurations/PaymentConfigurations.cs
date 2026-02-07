using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderPaymentSystem.DAL.Persistence.Constants;
using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.DAL.Persistence.Configurations;

internal class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
	public void Configure(EntityTypeBuilder<Payment> builder)
	{
		builder.HasKey(p => p.Id);

		builder.Property(p => p.AmountToPay)
			.IsRequired()
			.HasColumnType(DbColumnFormats.Decimal18_2);

		builder.Property(p => p.AmountPayed)
			.IsRequired()
			.HasColumnType(DbColumnFormats.Decimal18_2);

		builder.Property(p => p.CashChange)
			.HasColumnType(DbColumnFormats.Decimal18_2)
			.IsRequired(false);

		builder.Property(p => p.Method)
			.IsRequired()
			.HasConversion<string>();

		builder.Property(p => p.Status)
			.IsRequired()
			.HasConversion<string>();

		builder.HasOne(p => p.Order)
			.WithOne(o => o.Payment)
			.HasForeignKey<Payment>(p => p.OrderId)
			.OnDelete(DeleteBehavior.Cascade)
			.IsRequired();

		builder.Property(p => p.CreatedAt).IsRequired();
		builder.Property(p => p.UpdatedAt).IsRequired(false);
	}
}
