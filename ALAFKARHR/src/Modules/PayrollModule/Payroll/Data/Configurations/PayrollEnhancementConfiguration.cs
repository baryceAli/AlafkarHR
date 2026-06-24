using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payroll.Salaries.Models;

namespace Payroll.Data.Configurations;

public class SalaryStructureConfiguration : IEntityTypeConfiguration<SalaryStructure>
{
    public void Configure(EntityTypeBuilder<SalaryStructure> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.NameEng).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.SalaryStructureId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(x => x.Lines).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.HasIndex(x => new { x.CompanyId, x.Name }).IsUnique().HasFilter("[IsDeleted] = 0");
        builder.HasIndex(x => new { x.CompanyId, x.IsActive });
    }
}

public class SalaryStructureLineConfiguration : IEntityTypeConfiguration<SalaryStructureLine>
{
    public void Configure(EntityTypeBuilder<SalaryStructureLine> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ComponentType).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(x => x.Amount).HasPrecision(18, 2).IsRequired();
        builder.HasIndex(x => new { x.SalaryStructureId, x.ComponentId });
    }
}

public class SalaryStructureAssignmentConfiguration : IEntityTypeConfiguration<SalaryStructureAssignment>
{
    public void Configure(EntityTypeBuilder<SalaryStructureAssignment> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EffectiveFrom).IsRequired();
        builder.HasIndex(x => new { x.CompanyId, x.EmployeeId, x.IsActive });
        builder.HasIndex(x => new { x.CompanyId, x.SalaryStructureId });
    }
}

public class PayrollPeriodConfiguration : IEntityTypeConfiguration<PayrollPeriod>
{
    public void Configure(EntityTypeBuilder<PayrollPeriod> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Ignore(x => x.IsClosed);
        builder.HasIndex(x => new { x.CompanyId, x.Month, x.Year }).IsUnique().HasFilter("[IsDeleted] = 0");
        builder.HasIndex(x => new { x.CompanyId, x.Status });
    }
}

public class PayrollEntryConfiguration : IEntityTypeConfiguration<PayrollEntry>
{
    public void Configure(EntityTypeBuilder<PayrollEntry> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(x => x.GrossAmount).HasPrecision(18, 2);
        builder.Property(x => x.DeductionAmount).HasPrecision(18, 2);
        builder.Property(x => x.NetAmount).HasPrecision(18, 2);
        builder.Property(x => x.AccountingJournalNumber).HasMaxLength(100);
        builder.Ignore(x => x.IsPostedToAccounting);
        builder.HasIndex(x => new { x.CompanyId, x.PayrollPeriodId });
        builder.HasIndex(x => new { x.CompanyId, x.Status });
        builder.HasIndex(x => x.AccountingJournalEntryId);
    }
}

public class PayslipConfiguration : IEntityTypeConfiguration<Payslip>
{
    public void Configure(EntityTypeBuilder<Payslip> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(x => x.BasicAmount).HasPrecision(18, 2);
        builder.Property(x => x.TotalAllowances).HasPrecision(18, 2);
        builder.Property(x => x.TotalBenefits).HasPrecision(18, 2);
        builder.Property(x => x.TotalInputs).HasPrecision(18, 2);
        builder.Property(x => x.TotalDeductions).HasPrecision(18, 2);
        builder.Property(x => x.TotalLoans).HasPrecision(18, 2);
        builder.Property(x => x.GrossAmount).HasPrecision(18, 2);
        builder.Property(x => x.NetAmount).HasPrecision(18, 2);
        builder.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.PayslipId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(x => x.Lines).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.HasIndex(x => new { x.CompanyId, x.EmployeeId, x.PayrollPeriodId }).IsUnique().HasFilter("[IsDeleted] = 0");
        builder.HasIndex(x => new { x.PayrollEntryId, x.Status });
    }
}

public class PayslipLineConfiguration : IEntityTypeConfiguration<PayslipLine>
{
    public void Configure(EntityTypeBuilder<PayslipLine> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.NameEng).HasMaxLength(200);
        builder.Property(x => x.InputType).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.SourceType).HasMaxLength(100);
        builder.HasIndex(x => new { x.PayslipId, x.IsDeduction });
    }
}

public class PayrollInputConfiguration : IEntityTypeConfiguration<PayrollInput>
{
    public void Configure(EntityTypeBuilder<PayrollInput> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.InputType).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(x => x.Amount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(500);
        builder.HasIndex(x => new { x.CompanyId, x.EmployeeId, x.PayrollPeriodId });
        builder.HasIndex(x => new { x.CompanyId, x.IsProcessed });
    }
}

public class SaudiPayrollInfoConfiguration : IEntityTypeConfiguration<SaudiPayrollInfo>
{
    public void Configure(EntityTypeBuilder<SaudiPayrollInfo> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Iban).HasMaxLength(34);
        builder.Property(x => x.BankCode).HasMaxLength(50);
        builder.Property(x => x.BankName).HasMaxLength(200);
        builder.Property(x => x.GosiNumber).HasMaxLength(100);
        builder.Property(x => x.GosiEmployeePercentage).HasPrecision(5, 2);
        builder.Property(x => x.GosiEmployerPercentage).HasPrecision(5, 2);
        builder.Property(x => x.EosBasicSalary).HasPrecision(18, 2);
        builder.HasIndex(x => new { x.CompanyId, x.EmployeeId }).IsUnique().HasFilter("[IsDeleted] = 0");
        builder.HasIndex(x => new { x.CompanyId, x.IncludeInWps });
    }
}

public class WpsBatchConfiguration : IEntityTypeConfiguration<WpsBatch>
{
    public void Configure(EntityTypeBuilder<WpsBatch> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.BatchNumber).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(x => x.TotalAmount).HasPrecision(18, 2);
        builder.HasMany(x => x.Rows).WithOne().HasForeignKey(x => x.WpsBatchId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(x => x.Rows).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.HasIndex(x => new { x.CompanyId, x.PayrollPeriodId });
        builder.HasIndex(x => new { x.CompanyId, x.BatchNumber }).IsUnique().HasFilter("[IsDeleted] = 0");
    }
}

public class WpsBatchRowConfiguration : IEntityTypeConfiguration<WpsBatchRow>
{
    public void Configure(EntityTypeBuilder<WpsBatchRow> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Iban).HasMaxLength(34).IsRequired();
        builder.Property(x => x.BankCode).HasMaxLength(50);
        builder.Property(x => x.NetAmount).HasPrecision(18, 2);
        builder.Property(x => x.Remarks).HasMaxLength(500);
        builder.HasIndex(x => new { x.WpsBatchId, x.EmployeeId });
    }
}

public class EosProvisionSnapshotConfiguration : IEntityTypeConfiguration<EosProvisionSnapshot>
{
    public void Configure(EntityTypeBuilder<EosProvisionSnapshot> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.GrossPayBasis).HasPrecision(18, 2);
        builder.Property(x => x.ProvisionAmount).HasPrecision(18, 2);
        builder.Property(x => x.Notes).HasMaxLength(500);
        builder.HasIndex(x => new { x.CompanyId, x.EmployeeId, x.PayrollPeriodId });
    }
}

public class PayrollImportedWorkEntryConfiguration : IEntityTypeConfiguration<PayrollImportedWorkEntry>
{
    public void Configure(EntityTypeBuilder<PayrollImportedWorkEntry> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EntryType).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Hours).HasPrecision(18, 2);
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.Notes).HasMaxLength(500);
        builder.HasIndex(x => new { x.CompanyId, x.EmployeeId, x.PayrollPeriodId });
        builder.HasIndex(x => new { x.SourceWorkEntryId }).IsUnique().HasFilter("[SourceWorkEntryId] IS NOT NULL AND [IsDeleted] = 0");
    }
}
