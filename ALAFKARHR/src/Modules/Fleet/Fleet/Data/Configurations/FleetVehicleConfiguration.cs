namespace Fleet.Data.Configurations;

public class FleetVehicleConfiguration : IEntityTypeConfiguration<FleetVehicle>
{
    public void Configure(EntityTypeBuilder<FleetVehicle> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.Property(x => x.VehicleCode).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => x.VehicleCode).IsUnique();
        builder.Property(x => x.PlateNumber).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => x.PlateNumber).IsUnique();
        builder.Property(x => x.Name).HasMaxLength(250).IsRequired();
        builder.Property(x => x.NameEng).HasMaxLength(250).IsRequired();
        builder.Property(x => x.Make).HasMaxLength(120);
        builder.Property(x => x.Model).HasMaxLength(120);
        builder.Property(x => x.Color).HasMaxLength(80);
        builder.Property(x => x.Vin).HasMaxLength(120);
        builder.Property(x => x.EngineNumber).HasMaxLength(120);
        builder.Property(x => x.VehicleType).HasConversion<string>().HasMaxLength(40);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(40);
        builder.Property(x => x.OwnershipType).HasConversion<string>().HasMaxLength(40);
        builder.Property(x => x.PurchaseCost).HasPrecision(18, 2);
        builder.Property(x => x.MonthlyRent).HasPrecision(18, 2);
        builder.Property(x => x.DailyRent).HasPrecision(18, 2);
        builder.Property(x => x.DepositAmount).HasPrecision(18, 2);
        builder.Property(x => x.ExcessKilometerRate).HasPrecision(18, 2);
        builder.Property(x => x.FuelType).HasConversion<string>().HasMaxLength(40);
        builder.Property(x => x.FuelCapacity).HasPrecision(18, 2);
        builder.Property(x => x.Notes).HasMaxLength(2000);
        builder.HasIndex(x => x.CompanyId);
        builder.HasIndex(x => x.BranchId);
        builder.HasIndex(x => x.MaintenanceAssetId);
    }
}
