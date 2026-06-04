using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payroll.Salaries.Models;

namespace Payroll.Data.Configurations;

public class SalaryRunConfiguration : IEntityTypeConfiguration<SalaryRun>
{
    public void Configure(EntityTypeBuilder<SalaryRun> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.EmployeeId)
            .IsRequired();

        builder.Property(x => x.ContractId)
            .IsRequired();

        builder.Property(x => x.SalaryMonth)
            .IsRequired();

        builder.Property(x => x.SalaryYear)
            .IsRequired();

        builder.Property(x => x.totalSalary)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.TotalAllowances)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.totalDeductions)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.HasMany(x => x.SalaryRunItems)
            .WithOne()
            .HasForeignKey("SalaryRunId");

        builder.Navigation(x => x.SalaryRunItems)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(x => new { x.EmployeeId, x.SalaryMonth, x.SalaryYear })
            .IsUnique()
            .HasName("IX_SalaryRun_Employee_Month_Year");
    }
}
