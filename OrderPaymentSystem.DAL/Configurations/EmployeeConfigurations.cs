using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderPaymentSystem.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.DAL.Configurations
{
    public class EmployeeConfigurations : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.HasMany<Report>(x => x.Reports)
                .WithOne(x => x.Employee)
                .HasForeignKey(x => x.EmployeeId)
                .HasPrincipalKey(x => x.Id);

            builder.HasMany<Order>(x => x.Orders)
                .WithOne(x => x.Employee)
                .HasForeignKey(x => x.EmployeeId)
                .HasPrincipalKey(x => x.Id);
        }
    }
}
