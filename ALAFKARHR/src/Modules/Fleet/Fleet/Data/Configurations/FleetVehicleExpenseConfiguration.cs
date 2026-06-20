namespace Fleet.Data.Configurations;

public class FleetVehicleExpenseConfiguration : IEntityTypeConfiguration<FleetVehicleExpense>
{
    public void Configure(EntityTypeBuilder<FleetVehicleExpense> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.Property(x => x.Category).HasConversion<string>().HasMaxLength(60);
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.CurrencyId);
        builder.Property(x => x.CurrencyCode).HasMaxLength(10);
        builder.Property(x => x.VendorName).HasMaxLength(250);
        builder.Property(x => x.Quantity).HasPrecision(18, 3);
        builder.Property(x => x.UnitPrice).HasPrecision(18, 2);
        builder.Property(x => x.Notes).HasMaxLength(2000);
        builder.Property(x => x.FileName).HasMaxLength(250);
        builder.Property(x => x.FilePath).HasMaxLength(1000);
        builder.Property(x => x.ApprovalStatus).HasConversion<string>().HasMaxLength(40);
        builder.Property(x => x.ApprovalNotes).HasMaxLength(2000);
        builder.HasIndex(x => x.VehicleId);
        builder.HasIndex(x => x.CurrencyId);
        builder.HasIndex(x => x.ExpenseDate);
        builder.HasIndex(x => x.ApprovalStatus);
        builder.HasOne(x => x.Vehicle)
            .WithMany(x => x.Expenses)
            .HasForeignKey(x => x.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
