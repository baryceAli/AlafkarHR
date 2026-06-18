namespace Fleet.Data.Configurations;

public class FleetVehicleDocumentConfiguration : IEntityTypeConfiguration<FleetVehicleDocument>
{
    public void Configure(EntityTypeBuilder<FleetVehicleDocument> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.Property(x => x.DocumentType).HasConversion<string>().HasMaxLength(60);
        builder.Property(x => x.DocumentNumber).HasMaxLength(120);
        builder.Property(x => x.RenewalCost).HasPrecision(18, 2);
        builder.Property(x => x.FileName).HasMaxLength(250);
        builder.Property(x => x.FilePath).HasMaxLength(1000);
        builder.Property(x => x.ContentType).HasMaxLength(120);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(40);
        builder.Property(x => x.Notes).HasMaxLength(2000);
        builder.HasIndex(x => x.VehicleId);
        builder.HasIndex(x => x.ExpiryDate);
        builder.HasOne(x => x.Vehicle)
            .WithMany(x => x.Documents)
            .HasForeignKey(x => x.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
