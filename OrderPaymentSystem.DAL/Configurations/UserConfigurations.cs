using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderPaymentSystem.DAL.Helpers.Implementations;
using OrderPaymentSystem.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.DAL.Configurations
{
    public class UserConfigurations : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasData(new User
            {
                Id = 1,
                Login = "Maximlog",
                Password = "1234567",
                Email = "max_se@bk.ru",
                PhoneNumber = "+79493597126",
                CreatedAt = DateTime.UtcNow,
            },
            new User
            {
                Id = 2,
                Login = "SomeNewLogin",
                Password = "25252525",
                Email = "larisa_sed@mail.ru",
                PhoneNumber = "+79493612436",
                CreatedAt = DateTime.UtcNow,
            });

            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.Login).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Password).IsRequired();

            builder.Property(e => e.PhoneNumber)
                .HasMaxLength(25)
                .IsRequired()
                .HasConversion(phoneNumber => PhoneNumberValidation.FormatPhoneNumber(phoneNumber),
                dbPhoneNumber => dbPhoneNumber);

            builder.Property(e => e.Email)
                .HasMaxLength(100)
                .IsRequired()
                .HasConversion(email => EmailValidation.FormatEmail(email),
                dbEmail => dbEmail);

            builder.HasMany<Order>(x => x.Orders)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .HasPrincipalKey(x => x.Id);
        }
    }
}
