using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Helpers;

namespace OrderPaymentSystem.DAL.Configurations
{
    public class UserConfigurations : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasData(new User
            {
                Id = new Guid("0f8fad5b-d9cb-469f-a165-70867728950e"),
                Login = "Maximlog",
                Password = "BB864652C1B0118573EFC59CC0FF80FA083110D6BDABFA4C81B5515D3C7D1CB1-2BA5EE8C86EF4C052CFB4A79ECE2EAD7",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
            },
            new User
            {
                Id = new Guid("7c9e6679-7425-40de-944b-e07fc1f90ae7"),
                Login = "SomeNewLogin",
                Password = "196FB27D01E72EF1A92402BB0E5CF20BB56BC96F3182D20322D10E5DA79D022E-4411A9783D861D68C2807CEC937D4AEC",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
            });

            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.Login).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Password).IsRequired();

            builder.HasMany(x => x.Orders)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .HasPrincipalKey(x => x.Id);

            builder.HasOne(x => x.UserToken)
                    .WithOne(x => x.User)
                    .HasPrincipalKey<User>(x => x.Id)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Basket)
                    .WithOne(x => x.User)
                    .HasPrincipalKey<User>(x => x.Id)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Roles)
                .WithMany(x => x.Users)
                .UsingEntity<UserRole>(
                    l => l.HasOne<Role>().WithMany().HasForeignKey(x => x.RoleId),
                    l => l.HasOne<User>().WithMany().HasForeignKey(x => x.UserId)
                );
        }
    }
}
