namespace Inventory.Data.Configurations;

public class BarcodeOperationSessionConfiguration : IEntityTypeConfiguration<BarcodeOperationSession>
{
    public void Configure(EntityTypeBuilder<BarcodeOperationSession> builder)
    {
        builder.ToTable("BarcodeOperationSessions", "Inventory");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.OperationType).HasConversion<string>().HasMaxLength(40);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(40);
        builder.Property(x => x.SourceDocumentType).HasMaxLength(80);
        builder.Property(x => x.ReferenceNumber).HasMaxLength(120);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.Property(x => x.WarningsJson).HasMaxLength(2000);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);
        builder.HasIndex(x => new { x.CompanyId, x.OperationType, x.Status });
        builder.HasIndex(x => new { x.CompanyId, x.ReferenceNumber });
        builder.HasIndex(x => x.InventoryOperationId);
        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.HasMany(x => x.Lines)
            .WithOne()
            .HasForeignKey(x => x.BarcodeOperationSessionId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(x => x.Lines)
            .HasField("_lines")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class BarcodeOperationLineConfiguration : IEntityTypeConfiguration<BarcodeOperationLine>
{
    public void Configure(EntityTypeBuilder<BarcodeOperationLine> builder)
    {
        builder.ToTable("BarcodeOperationLines", "Inventory");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.RawBarcode).IsRequired().HasMaxLength(180);
        builder.Property(x => x.EntityType).HasConversion<string>().HasMaxLength(40);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(40);
        builder.Property(x => x.EnteredQuantity).HasPrecision(18, 4);
        builder.Property(x => x.PackageMultiplier).HasPrecision(18, 4);
        builder.Property(x => x.UnitMultiplier).HasPrecision(18, 4);
        builder.Property(x => x.NormalizedQuantity).HasPrecision(18, 4);
        builder.Property(x => x.DisplayLabel).HasMaxLength(250);
        builder.Property(x => x.DisplayLabelEng).HasMaxLength(250);
        builder.Property(x => x.BatchNumber).HasMaxLength(120);
        builder.Property(x => x.SerialNumber).HasMaxLength(120);
        builder.Property(x => x.Warning).HasMaxLength(1000);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);
        builder.HasIndex(x => new { x.BarcodeOperationSessionId, x.EntityType });
        builder.HasIndex(x => x.ProductSkuId);
        builder.HasIndex(x => x.BatchId);
        builder.HasIndex(x => x.InventorySerialNumberId);
        builder.HasIndex(x => x.InventoryOperationLineId);
        builder.HasIndex(x => x.SerialNumber);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
