using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderPaymentSystem.Domain.Entities;
using static OrderPaymentSystem.Domain.Constants.ValidationConstants.Role;

namespace OrderPaymentSystem.DAL.Persistence.Configurations;

internal class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(MaxNameLength);
    }
}
