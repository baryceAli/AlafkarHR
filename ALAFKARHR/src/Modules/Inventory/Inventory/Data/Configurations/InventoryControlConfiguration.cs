namespace Inventory.Data.Configurations;

public class WarehouseLocationConfiguration : IEntityTypeConfiguration<WarehouseLocation>
{
    public void Configure(EntityTypeBuilder<WarehouseLocation> builder)
    {
        builder.ToTable("WarehouseLocations", "Inventory");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).IsRequired().HasMaxLength(80);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.NameEng).HasMaxLength(200);
        builder.Property(x => x.ParentCode).HasMaxLength(80);
        builder.Property(x => x.LocationType).HasConversion<string>().HasMaxLength(40);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);
        builder.HasIndex(x => new { x.CompanyId, x.WarehouseId, x.Code }).IsUnique();
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class InventoryLocationBalanceConfiguration : IEntityTypeConfiguration<InventoryLocationBalance>
{
    public void Configure(EntityTypeBuilder<InventoryLocationBalance> builder)
    {
        builder.ToTable("InventoryLocationBalances", "Inventory");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Quantity).HasPrecision(18, 4);
        builder.Property(x => x.ReservedQuantity).HasPrecision(18, 4);
        builder.Property(x => x.RowVersion).IsRowVersion();
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);
        builder.HasIndex(x => new { x.CompanyId, x.WarehouseId, x.WarehouseLocationId, x.ProductSkuId, x.BatchId })
            .IsUnique()
            .HasDatabaseName("UX_InventoryLocationBalance_Key");
        builder.HasIndex(x => new { x.CompanyId, x.ProductSkuId, x.WarehouseId });
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class CycleCountConfiguration : IEntityTypeConfiguration<CycleCount>
{
    public void Configure(EntityTypeBuilder<CycleCount> builder)
    {
        builder.ToTable("CycleCounts", "Inventory");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CountNumber).IsRequired().HasMaxLength(120);
        builder.Property(x => x.Reason).HasMaxLength(500);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);
        builder.HasIndex(x => new { x.CompanyId, x.CountNumber }).IsUnique();
        builder.HasIndex(x => new { x.CompanyId, x.WarehouseId, x.WarehouseLocationId, x.CountDate });
        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.HasMany(x => x.Lines)
            .WithOne()
            .HasForeignKey(x => x.CycleCountId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(x => x.Lines)
            .HasField("_lines")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class CycleCountLineConfiguration : IEntityTypeConfiguration<CycleCountLine>
{
    public void Configure(EntityTypeBuilder<CycleCountLine> builder)
    {
        builder.ToTable("CycleCountLines", "Inventory");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CountedQuantity).HasPrecision(18, 4);
        builder.Property(x => x.SerialNumbersCsv).HasMaxLength(4000);
        builder.Property(x => x.Notes).HasMaxLength(500);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);
        builder.HasIndex(x => new { x.CycleCountId, x.ProductSkuId, x.BatchId });
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class PutawayRuleConfiguration : IEntityTypeConfiguration<PutawayRule>
{
    public void Configure(EntityTypeBuilder<PutawayRule> builder)
    {
        builder.ToTable("PutawayRules", "Inventory");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.RemovalStrategy).HasConversion<string>().HasMaxLength(40);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);
        builder.HasIndex(x => new { x.CompanyId, x.WarehouseId, x.ProductSkuId, x.Priority });
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class QualityInspectionConfiguration : IEntityTypeConfiguration<QualityInspection>
{
    public void Configure(EntityTypeBuilder<QualityInspection> builder)
    {
        builder.ToTable("QualityInspections", "Inventory");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.SourceDocumentNumber).HasMaxLength(100);
        builder.Property(x => x.Quantity).HasPrecision(18, 4);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(40);
        builder.Property(x => x.ResultNotes).HasMaxLength(2000);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);
        builder.HasIndex(x => new { x.CompanyId, x.SourceDocumentId, x.ProductSkuId });
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class LandedCostVoucherConfiguration : IEntityTypeConfiguration<LandedCostVoucher>
{
    public void Configure(EntityTypeBuilder<LandedCostVoucher> builder)
    {
        builder.ToTable("LandedCostVouchers", "Inventory");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.SourceDocumentNumber).IsRequired().HasMaxLength(100);
        builder.Property(x => x.AllocationMethod).HasConversion<string>().HasMaxLength(40);
        builder.Property(x => x.FreightAmount).HasPrecision(18, 2);
        builder.Property(x => x.CustomsAmount).HasPrecision(18, 2);
        builder.Property(x => x.HandlingAmount).HasPrecision(18, 2);
        builder.Property(x => x.OtherAmount).HasPrecision(18, 2);
        builder.Property(x => x.TotalAmount).HasPrecision(18, 2);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);
        builder.HasIndex(x => new { x.CompanyId, x.SourceDocumentId });
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class InventoryValuationLayerConfiguration : IEntityTypeConfiguration<InventoryValuationLayer>
{
    public void Configure(EntityTypeBuilder<InventoryValuationLayer> builder)
    {
        builder.ToTable("InventoryValuationLayers", "Inventory");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.SourceDocumentType).IsRequired().HasMaxLength(80);
        builder.Property(x => x.ReferenceNumber).IsRequired().HasMaxLength(120);
        builder.Property(x => x.Quantity).HasPrecision(18, 4);
        builder.Property(x => x.UnitCost).HasPrecision(18, 4);
        builder.Property(x => x.TotalCost).HasPrecision(18, 2);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);
        builder.HasIndex(x => new { x.CompanyId, x.ProductSkuId, x.WarehouseId, x.LayerDate });
    }
}
