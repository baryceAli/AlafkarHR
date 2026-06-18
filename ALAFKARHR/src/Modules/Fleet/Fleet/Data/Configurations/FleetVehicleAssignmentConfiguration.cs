namespace Fleet.Data.Configurations;

public class FleetVehicleAssignmentConfiguration : IEntityTypeConfiguration<FleetVehicleAssignment>
{
    public void Configure(EntityTypeBuilder<FleetVehicleAssignment> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.Property(x => x.Purpose).HasMaxLength(500);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(40);
        builder.Property(x => x.FuelLevelOut).HasPrecision(5, 2);
        builder.Property(x => x.FuelLevelIn).HasPrecision(5, 2);
        builder.HasIndex(x => x.VehicleId);
        builder.HasIndex(x => x.Status);
        builder.HasOne(x => x.Vehicle)
            .WithMany(x => x.Assignments)
            .HasForeignKey(x => x.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
