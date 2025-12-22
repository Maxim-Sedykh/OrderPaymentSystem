using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.DAL.Configurations;

public class UserConfigurations : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
		builder.HasKey(u => u.Id);

		builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Login).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Password).IsRequired();

        builder.HasOne(x => x.UserToken)
                .WithOne(x => x.User)
                .HasPrincipalKey<User>(x => x.Id)
                .OnDelete(DeleteBehavior.Cascade);

		builder.HasMany(u => u.Orders)
			.WithOne(o => o.User)
			.HasForeignKey(o => o.UserId)
			.OnDelete(DeleteBehavior.Restrict); // Заказы не должны удаляться при удалении пользователя

		// Связь один-ко-многим с BasketItem
		builder.HasMany(u => u.BasketItems)
			.WithOne(bi => bi.User)
			.HasForeignKey(bi => bi.UserId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasMany(x => x.Roles)
            .WithMany(x => x.Users)
            .UsingEntity<UserRole>(
                l => l.HasOne<Role>().WithMany().HasForeignKey(x => x.RoleId),
                l => l.HasOne<User>().WithMany().HasForeignKey(x => x.UserId)
            );
    }
}
