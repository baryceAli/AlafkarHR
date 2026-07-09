namespace Inventory.Data.Configurations;

public class ScrapOrderConfiguration : IEntityTypeConfiguration<ScrapOrder>
{
    public void Configure(EntityTypeBuilder<ScrapOrder> builder)
    {
        builder.ToTable("ScrapOrders", "Inventory");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ScrapOrderNumber).IsRequired().HasMaxLength(120);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(40);
        builder.Property(x => x.SourceDocumentType).HasMaxLength(80);
        builder.Property(x => x.SourceDocumentNumber).HasMaxLength(120);
        builder.Property(x => x.Reason).HasMaxLength(300);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.Property(x => x.ValidatedBy).HasMaxLength(100);
        builder.Property(x => x.CancelledBy).HasMaxLength(100);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);
        builder.HasIndex(x => new { x.CompanyId, x.ScrapOrderNumber }).IsUnique();
        builder.HasIndex(x => new { x.CompanyId, x.BranchId, x.Status });
        builder.HasIndex(x => new { x.CompanyId, x.WarehouseId, x.Status });
        builder.HasIndex(x => new { x.SourceDocumentType, x.SourceDocumentId });
        builder.HasIndex(x => x.SourceInventoryOperationId);
        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.HasMany(x => x.Lines)
            .WithOne()
            .HasForeignKey(x => x.ScrapOrderId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(x => x.Lines)
            .HasField("_lines")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class ScrapOrderLineConfiguration : IEntityTypeConfiguration<ScrapOrderLine>
{
    public void Configure(EntityTypeBuilder<ScrapOrderLine> builder)
    {
        builder.ToTable("ScrapOrderLines", "Inventory");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Quantity).HasPrecision(18, 4);
        builder.Property(x => x.UnitCost).HasPrecision(18, 4);
        builder.Property(x => x.TotalCost).HasPrecision(18, 2);
        builder.Property(x => x.Reason).HasMaxLength(300);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);
        builder.HasIndex(x => new { x.ScrapOrderId, x.LineNumber });
        builder.HasIndex(x => new { x.ProductSkuId, x.BatchId });
        builder.HasIndex(x => x.SourceDocumentLineId);
        builder.HasIndex(x => x.SourceInventoryOperationLineId);
        builder.HasIndex(x => x.StockMovementId);
        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.HasMany(x => x.Serials)
            .WithOne()
            .HasForeignKey(x => x.ScrapOrderLineId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(x => x.Serials)
            .HasField("_serials")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class ScrapOrderLineSerialConfiguration : IEntityTypeConfiguration<ScrapOrderLineSerial>
{
    public void Configure(EntityTypeBuilder<ScrapOrderLineSerial> builder)
    {
        builder.ToTable("ScrapOrderLineSerials", "Inventory");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.SerialNumber).IsRequired().HasMaxLength(120);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);
        builder.HasIndex(x => new { x.ScrapOrderLineId, x.SerialNumber }).IsUnique();
        builder.HasIndex(x => x.InventorySerialNumberId);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
