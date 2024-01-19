using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.DAL.Configurations
{
    public class ReportConfigurations : IEntityTypeConfiguration<Report>
    {
        public void Configure(EntityTypeBuilder<Report> builder)
        {
            builder.HasData(new Report
            {
                Id = 1,
                Name = "Первый отчёт",
                TotalRevenues = 30,
                NumberOfOrders = 24,
                EmployeeId = 1,
                CreatedAt = DateTime.UtcNow,
            },
            new Report
            {
                Id = 2,
                Name = "Второй отчёт",
                TotalRevenues = 60,
                NumberOfOrders = 33,
                EmployeeId = 2,
                CreatedAt = DateTime.UtcNow,
            });

            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        }
    }
}
