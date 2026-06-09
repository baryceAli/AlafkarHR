using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payroll.Salaries.Models;

namespace Payroll.Data.Configurations;

public class SalaryRunItemConfiguration : IEntityTypeConfiguration<SalaryRunItem>
{
    public void Configure(EntityTypeBuilder<SalaryRunItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.SalaryRunId)
            .IsRequired();

        builder.Property(x => x.ItemId)
            .IsRequired();

        builder.Property(x => x.ComponentType)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(x => x.Amount)
            .HasPrecision(18, 2)
            .IsRequired();
    }
}
