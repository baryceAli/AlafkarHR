using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payroll.Salaries.Models;

namespace Payroll.Data.Configurations;

public class EmployeeLoanConfiguration : IEntityTypeConfiguration<EmployeeLoan>
{
    public void Configure(EntityTypeBuilder<EmployeeLoan> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CompanyId).IsRequired();
        builder.Property(x => x.EmployeeId).IsRequired();

        builder.Property(x => x.Type)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(x => x.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.InstallmentAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.DeductedAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.StartMonth).IsRequired();
        builder.Property(x => x.StartYear).IsRequired();

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(x => x.ReferenceNo)
            .HasMaxLength(128);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.Ignore(x => x.RemainingAmount);

        builder.HasIndex(x => new { x.CompanyId, x.EmployeeId, x.Status });
    }
}
