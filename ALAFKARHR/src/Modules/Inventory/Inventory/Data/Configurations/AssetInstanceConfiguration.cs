using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Data.Configurations;

public class AssetInstanceConfiguration : IEntityTypeConfiguration<AssetInstance>
{
    public void Configure(EntityTypeBuilder<AssetInstance> builder)
    {
        builder.ToTable("AssetInstances", "Inventory");
        builder.HasKey(x => x.Id);
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.AssetTag).HasMaxLength(80).IsRequired();
        builder.Property(x => x.SerialNumber).HasMaxLength(120);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(40);
        builder.Property(x => x.Notes).HasMaxLength(2000);

        builder.HasIndex(x => new { x.CompanyId, x.AssetTag }).IsUnique();
        builder.HasIndex(x => new { x.CompanyId, x.SerialNumber })
            .IsUnique()
            .HasFilter("[SerialNumber] IS NOT NULL");
        builder.HasIndex(x => x.ProductSkuId);
        builder.HasIndex(x => x.BranchId);
        builder.HasIndex(x => x.DepartmentId);
        builder.HasIndex(x => x.EmployeeId);
        builder.HasIndex(x => x.WarehouseId);
        builder.HasIndex(x => x.MaintenanceAssetId);
    }
}
