namespace Fleet.Data.Configurations;

public class FleetVehicleServiceRuleConfiguration : IEntityTypeConfiguration<FleetVehicleServiceRule>
{
    public void Configure(EntityTypeBuilder<FleetVehicleServiceRule> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.Property(x => x.ServiceType).HasConversion<string>().HasMaxLength(60);
        builder.Property(x => x.Notes).HasMaxLength(2000);
        builder.HasIndex(x => x.VehicleId);
        builder.HasIndex(x => x.NextDueDate);
        builder.HasIndex(x => x.NextDueOdometer);
        builder.HasOne(x => x.Vehicle)
            .WithMany(x => x.ServiceRules)
            .HasForeignKey(x => x.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
