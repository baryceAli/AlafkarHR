using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payroll.Salaries.Models;

namespace Payroll.Data.Configurations;

public class ContractItemConfiguration : IEntityTypeConfiguration<ContractItem>
{
    public void Configure(EntityTypeBuilder<ContractItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ContractId)
            .IsRequired();

        builder.Property(x => x.ComponentId)
            .IsRequired();

        builder.Property(x => x.Amount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.CompanyId)
            .IsRequired();
    }
}
