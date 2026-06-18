namespace Maintenance.Data.Configurations;

public class MaintenanceAssetConfiguration : IEntityTypeConfiguration<MaintenanceAsset>
{
    public void Configure(EntityTypeBuilder<MaintenanceAsset> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.Property(x => x.AssetCode).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => x.AssetCode).IsUnique();
        builder.Property(x => x.Name).HasMaxLength(250).IsRequired();
        builder.Property(x => x.NameEng).HasMaxLength(250).IsRequired();
        builder.Property(x => x.AssetType).HasConversion<string>().HasMaxLength(40);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(40);
        builder.Property(x => x.Description).HasMaxLength(2000);
        builder.Property(x => x.SourceModule).HasMaxLength(100);
        builder.Property(x => x.SourceEntityName).HasMaxLength(150);
        builder.Property(x => x.Location).HasMaxLength(500);
        builder.Property(x => x.SerialNumber).HasMaxLength(120);
        builder.HasIndex(x => x.CompanyId);
        builder.HasIndex(x => x.BranchId);
        builder.HasIndex(x => x.ParentAssetId);
        builder.HasIndex(x => new { x.SourceModule, x.SourceEntityName, x.SourceEntityId })
            .IsUnique()
            .HasFilter("[SourceModule] IS NOT NULL AND [SourceEntityName] IS NOT NULL AND [SourceEntityId] IS NOT NULL");
        builder.HasOne(x => x.ParentAsset)
            .WithMany(x => x.ChildAssets)
            .HasForeignKey(x => x.ParentAssetId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
