namespace Inventory.Data.Configurations;

public class PickingGroupConfiguration : IEntityTypeConfiguration<PickingGroup>
{
    public void Configure(EntityTypeBuilder<PickingGroup> builder)
    {
        builder.ToTable("PickingGroups", "Inventory");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.GroupNumber).IsRequired().HasMaxLength(120);
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.GroupType).HasConversion<string>().HasMaxLength(40);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(40);
        builder.Property(x => x.ResponsibleUserId).HasMaxLength(100);
        builder.Property(x => x.DockLocation).HasMaxLength(120);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.Property(x => x.ProcessedBy).HasMaxLength(100);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);
        builder.HasIndex(x => new { x.CompanyId, x.GroupNumber }).IsUnique();
        builder.HasIndex(x => new { x.CompanyId, x.BranchId, x.Status });
        builder.HasIndex(x => new { x.CompanyId, x.WarehouseId, x.GroupType, x.Status });
        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.HasMany(x => x.Lines)
            .WithOne()
            .HasForeignKey(x => x.PickingGroupId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(x => x.Lines)
            .HasField("_lines")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class PickingGroupLineConfiguration : IEntityTypeConfiguration<PickingGroupLine>
{
    public void Configure(EntityTypeBuilder<PickingGroupLine> builder)
    {
        builder.ToTable("PickingGroupLines", "Inventory");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.SourceDocumentType).IsRequired().HasMaxLength(80);
        builder.Property(x => x.SourceDocumentNumber).IsRequired().HasMaxLength(120);
        builder.Property(x => x.OperationKind).HasConversion<string>().HasMaxLength(40);
        builder.Property(x => x.OperationStatus).HasConversion<string>().HasMaxLength(40);
        builder.Property(x => x.PlannedQuantity).HasPrecision(18, 4);
        builder.Property(x => x.DoneQuantity).HasPrecision(18, 4);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);
        builder.HasIndex(x => new { x.PickingGroupId, x.InventoryOperationId }).IsUnique();
        builder.HasIndex(x => x.InventoryOperationId);
        builder.HasIndex(x => new { x.SourceDocumentType, x.SourceDocumentId });
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
