using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payroll.Salaries.Models;

namespace Payroll.Data.Configurations;

public class EmployeeAllowanceConfiguration : IEntityTypeConfiguration<EmployeeAllowance>
{
    public void Configure(EntityTypeBuilder<EmployeeAllowance> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.EmployeeId)
            .IsRequired();

        builder.Property(x => x.ComponentId)
            .IsRequired();

        builder.Property(x => x.CalculationType)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(x => x.Value)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.IsRecurring)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();
    }
}
