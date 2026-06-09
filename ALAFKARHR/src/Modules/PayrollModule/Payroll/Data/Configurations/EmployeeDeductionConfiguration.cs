using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payroll.Salaries.Models;

namespace Payroll.Data.Configurations;

public class EmployeeDeductionConfiguration : IEntityTypeConfiguration<EmployeeDeduction>
{
    public void Configure(EntityTypeBuilder<EmployeeDeduction> builder)
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

        builder.Property(x => x.TotalAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.InstallmentAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.DeductedAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.IsActive)
            .IsRequired();
    }
}
