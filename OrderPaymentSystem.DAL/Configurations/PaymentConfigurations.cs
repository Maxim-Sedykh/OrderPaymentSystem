using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.DAL.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
	public void Configure(EntityTypeBuilder<Payment> builder)
	{
		builder.HasKey(p => p.Id);

		builder.Property(p => p.AmountDue)
			.IsRequired()
			.HasColumnType("decimal(18,2)");

		builder.Property(p => p.AmountPaid)
			.IsRequired()
			.HasColumnType("decimal(18,2)");

		builder.Property(p => p.CashChange)
			.HasColumnType("decimal(18,2)")
			.IsRequired(false); // Сдача может быть null

		builder.Property(p => p.PaymentMethod)
			.IsRequired()
			.HasConversion<string>(); // Храним enum как строку в БД

		builder.Property(p => p.Status)
			.IsRequired()
			.HasConversion<string>(); // Храним enum как строку в БД

		// Связь один-к-одному с Order
		builder.HasOne(p => p.Order)
			.WithOne(o => o.Payment)
			.HasForeignKey<Payment>(p => p.OrderId)
			.OnDelete(DeleteBehavior.Cascade)// Внешний ключ в сущности Payment
			.IsRequired(); // Платеж должен быть привязан к заказу
						   // .OnDelete(DeleteBehavior.Restrict) // Удаление заказа при наличии платежа будет заблокировано.
						   // Или Cascade, если удаление заказа всегда должно удалять платеж.
						   // Выбирайте в зависимости от бизнес-логики. Restrict - более безопасный вариант.
						   // Важно: если Order.PaymentId является nullable, а Payment.OrderId IsRequired,
						   // то при удалении Order, PaymentId в Order будет null, но Payment.OrderId останется.
						   // Если хотим, чтобы при удалении Order платеж не удалялся, но его связь с Order разрывалась,
						   // то Order.PaymentId должен быть nullable, и OnDelete(DeleteBehavior.SetNull) на Order стороне
						   // для свойства PaymentId. Но так как PaymentId у Payment, то тут SetNull не применить.
						   // Лучше оставить Restrict, чтобы гарантировать целостность, пока не решится вопрос с платежом.
						   // Для большинства сценариев Restrict - хороший дефолт.

		// Аудит-поля
		builder.Property(p => p.CreatedAt).IsRequired();
		builder.Property(p => p.CreatedBy).IsRequired();
		builder.Property(p => p.UpdatedAt).IsRequired(false);
		builder.Property(p => p.UpdatedBy).IsRequired(false);
	}
}
