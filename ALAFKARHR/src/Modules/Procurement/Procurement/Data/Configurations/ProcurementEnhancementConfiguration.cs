namespace Procurement.Data.Configurations;

public class SupplierItemConfiguration : IEntityTypeConfiguration<SupplierItem>
{
    public void Configure(EntityTypeBuilder<SupplierItem> builder)
    {
        builder.ToTable("SupplierItems");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.SupplierName).HasMaxLength(200);
        builder.Property(x => x.ProductName).HasMaxLength(200);
        builder.Property(x => x.ProductNameEng).HasMaxLength(200);
        builder.Property(x => x.SkuCode).HasMaxLength(100);
        builder.Property(x => x.SupplierSku).HasMaxLength(100);
        builder.Property(x => x.MinimumOrderQuantity).HasPrecision(18, 4);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);
        builder.HasIndex(x => new { x.CompanyId, x.SupplierId, x.ProductSkuId, x.SupplierSku });
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class VendorPricelistConfiguration : IEntityTypeConfiguration<VendorPricelist>
{
    public void Configure(EntityTypeBuilder<VendorPricelist> builder)
    {
        builder.ToTable("VendorPricelists");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.SupplierName).HasMaxLength(200);
        builder.Property(x => x.MinimumQuantity).HasPrecision(18, 4);
        builder.Property(x => x.UnitCost).HasPrecision(18, 4);
        builder.Property(x => x.DiscountRate).HasPrecision(18, 4);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);
        builder.HasIndex(x => new { x.CompanyId, x.SupplierId, x.ProductSkuId, x.ValidFrom });
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class ReorderingRuleConfiguration : IEntityTypeConfiguration<ReorderingRule>
{
    public void Configure(EntityTypeBuilder<ReorderingRule> builder)
    {
        builder.ToTable("ReorderingRules");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.MinimumQuantity).HasPrecision(18, 4);
        builder.Property(x => x.MaximumQuantity).HasPrecision(18, 4);
        builder.Property(x => x.ReorderQuantity).HasPrecision(18, 4);
        builder.Property(x => x.MultipleQuantity).HasPrecision(18, 4);
        builder.Property(x => x.TriggerMode)
            .HasConversion<string>()
            .HasMaxLength(40)
            .HasDefaultValue(ReplenishmentTriggerMode.Manual);
        builder.Property(x => x.LastGeneratedDocumentNumber).HasMaxLength(100);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);
        builder.HasIndex(x => new { x.CompanyId, x.ProductSkuId, x.WarehouseId });
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
