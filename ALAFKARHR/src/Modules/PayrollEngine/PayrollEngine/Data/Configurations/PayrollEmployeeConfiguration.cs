using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayrollEngine.Payroll.Models;

namespace PayrollEngine.Data.Configurations;
public class PayrollEmployeeConfiguration : IEntityTypeConfiguration<PayrollEmployee>
{
    public void Configure(EntityTypeBuilder<PayrollEmployee> builder)
    {
        builder.ToTable("PayrollEmployees");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.BasicSalary).HasPrecision(18, 2);
        builder.Property(x => x.NetSalary).HasPrecision(18, 2);

        builder.HasIndex(x => x.EmployeeId);
        builder.HasIndex(x => x.PayrollRunId);
    }
}