using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Data.Configurations;

public class WarehouseTransferConfiguration : IEntityTypeConfiguration<WarehouseTransfer>
{
    public void Configure(EntityTypeBuilder<WarehouseTransfer> builder)
    {
        builder.ToTable("WarehouseTransfers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.SourceWarehouseId)
            .IsRequired();

        builder.Property(x => x.DestinationWarehouseId)
            .IsRequired();

        builder.Property(x => x.CompanyId)
            .IsRequired();

        builder.Property(x => x.TransferNumber)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(x => x.ReferenceNumber)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(x => x.Reason)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.ShippedAt);

        builder.Property(x => x.ReceivedAt);

        // Audit Fields
        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.ModifiedAt);

        builder.Property(x => x.ModifiedBy)
            .HasMaxLength(100);

        // Items
        builder.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey("WarehouseTransferId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Items)
            .UsePropertyAccessMode(
                PropertyAccessMode.Field);

        // Indexes
        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => x.SourceWarehouseId);

        builder.HasIndex(x => x.DestinationWarehouseId);

        builder.HasIndex(x => new { x.CompanyId, x.TransferNumber })
            .IsUnique()
            .HasDatabaseName("UX_WarehouseTransfer_Company_TransferNumber");
    }
}
