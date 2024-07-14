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
                Password = HashPasswordHelper.HashPassword("1234567"),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
            },
            new User
            {
                Id = new Guid("7c9e6679-7425-40de-944b-e07fc1f90ae7"),
                Login = "SomeNewLogin",
                Password = HashPasswordHelper.HashPassword("25252525"),
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
