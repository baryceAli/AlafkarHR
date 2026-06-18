namespace RealEstate.Data.Configurations;

public class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.ToTable("Properties");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).HasMaxLength(50);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.NameEng).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Address).HasMaxLength(500);
        builder.Property(x => x.City).HasMaxLength(100);
        builder.Property(x => x.District).HasMaxLength(100);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasIndex(x => new { x.CompanyId, x.Code });
        builder.HasIndex(x => new { x.CompanyId, x.Status });
        builder.HasMany(x => x.Units)
            .WithOne(x => x.Property)
            .HasForeignKey(x => x.PropertyId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Navigation(x => x.Units).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class PropertyUnitConfiguration : IEntityTypeConfiguration<PropertyUnit>
{
    public void Configure(EntityTypeBuilder<PropertyUnit> builder)
    {
        builder.ToTable("PropertyUnits");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.UnitNumber).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.Floor).HasMaxLength(50);
        builder.Property(x => x.Area).HasPrecision(18, 2);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasIndex(x => new { x.PropertyId, x.UnitNumber });
        builder.HasIndex(x => new { x.PropertyId, x.Status });
    }
}

public class LeaseConfiguration : IEntityTypeConfiguration<Lease>
{
    public void Configure(EntityTypeBuilder<Lease> builder)
    {
        builder.ToTable("Leases");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Number).HasMaxLength(50).IsRequired();
        builder.Property(x => x.ContractNumber).HasMaxLength(100);
        builder.Property(x => x.PartyType).HasMaxLength(50).IsRequired();
        builder.Property(x => x.PartyDisplayName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.RentAmount).HasPrecision(18, 2);
        builder.Property(x => x.DepositAmount).HasPrecision(18, 2);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasIndex(x => new { x.CompanyId, x.Number }).IsUnique();
        builder.HasIndex(x => new { x.Direction, x.Status, x.PropertyId, x.UnitId, x.StartDate, x.EndDate });
        builder.HasOne(x => x.Property).WithMany().HasForeignKey(x => x.PropertyId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Unit).WithMany().HasForeignKey(x => x.UnitId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Installments)
            .WithOne()
            .HasForeignKey(x => x.LeaseId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(x => x.Installments).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class LeaseInstallmentConfiguration : IEntityTypeConfiguration<LeaseInstallment>
{
    public void Configure(EntityTypeBuilder<LeaseInstallment> builder)
    {
        builder.ToTable("LeaseInstallments");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.PaidAmount).HasPrecision(18, 2);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.Ignore(x => x.RemainingAmount);
        builder.HasIndex(x => new { x.LeaseId, x.Sequence });
        builder.HasIndex(x => new { x.Status, x.DueDate });
        builder.HasMany(x => x.Allocations)
            .WithOne()
            .HasForeignKey(x => x.LeaseInstallmentId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(x => x.Allocations).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class LeasePaymentAllocationConfiguration : IEntityTypeConfiguration<LeasePaymentAllocation>
{
    public void Configure(EntityTypeBuilder<LeasePaymentAllocation> builder)
    {
        builder.ToTable("LeasePaymentAllocations");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Reference).HasMaxLength(100);
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasIndex(x => new { x.LeaseInstallmentId, x.PaymentReferenceId });
    }
}

public class PropertyExpenseConfiguration : IEntityTypeConfiguration<PropertyExpense>
{
    public void Configure(EntityTypeBuilder<PropertyExpense> builder)
    {
        builder.ToTable("PropertyExpenses");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.TaxAmount).HasPrecision(18, 2);
        builder.Ignore(x => x.TotalAmount);
        builder.Property(x => x.SupplierName).HasMaxLength(200);
        builder.Property(x => x.SourceDocumentNumber).HasMaxLength(100);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasOne(x => x.Property).WithMany().HasForeignKey(x => x.PropertyId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.CompanyId, x.PropertyId, x.Category, x.ExpenseDate });
    }
}

public class UtilityAccountConfiguration : IEntityTypeConfiguration<UtilityAccount>
{
    public void Configure(EntityTypeBuilder<UtilityAccount> builder)
    {
        builder.ToTable("UtilityAccounts");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.AccountNumber).HasMaxLength(100).IsRequired();
        builder.Property(x => x.MeterNumber).HasMaxLength(100);
        builder.Property(x => x.ProviderName).HasMaxLength(200);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasIndex(x => new { x.PropertyId, x.UnitId, x.ServiceType, x.AccountNumber });
    }
}

public class UtilityBillConfiguration : IEntityTypeConfiguration<UtilityBill>
{
    public void Configure(EntityTypeBuilder<UtilityBill> builder)
    {
        builder.ToTable("UtilityBills");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.TaxAmount).HasPrecision(18, 2);
        builder.Ignore(x => x.TotalAmount);
        builder.Property(x => x.Reference).HasMaxLength(100);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasIndex(x => new { x.UtilityAccountId, x.DueDate, x.IsPaid });
    }
}

public class OccupancyHistoryConfiguration : IEntityTypeConfiguration<OccupancyHistory>
{
    public void Configure(EntityTypeBuilder<OccupancyHistory> builder)
    {
        builder.ToTable("OccupancyHistory");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.UnitId, x.StartDate, x.EndDate });
        builder.HasIndex(x => new { x.LeaseId });
    }
}
