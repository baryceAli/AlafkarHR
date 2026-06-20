namespace Maintenance.Data.Configurations;

public class MaintenanceWorkOrderConfiguration : IEntityTypeConfiguration<MaintenanceWorkOrder>
{
    public void Configure(EntityTypeBuilder<MaintenanceWorkOrder> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.Property(x => x.WorkOrderNumber).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => x.WorkOrderNumber).IsUnique();
        builder.Property(x => x.Title).HasMaxLength(250).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(4000);
        builder.Property(x => x.AssignedToUserId).HasMaxLength(120);
        builder.Property(x => x.Priority).HasConversion<string>().HasMaxLength(40);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(40);
        builder.Property(x => x.Category).HasMaxLength(120);
        builder.Property(x => x.InternalNotes).HasMaxLength(2000);
        builder.Property(x => x.EstimatedCost).HasPrecision(18, 2);
        builder.Property(x => x.ApprovedCost).HasPrecision(18, 2);
        builder.Property(x => x.ActualCost).HasPrecision(18, 2);
        builder.Property(x => x.CurrencyId);
        builder.Property(x => x.CurrencyCode).HasMaxLength(10);
        builder.Property(x => x.VendorName).HasMaxLength(250);
        builder.Property(x => x.CostApprovalStatus).HasConversion<string>().HasMaxLength(40);
        builder.Property(x => x.ApprovalNotes).HasMaxLength(2000);
        builder.HasIndex(x => x.AssetId);
        builder.HasIndex(x => x.CurrencyId);
        builder.HasIndex(x => x.RequestedByUserId);
        builder.HasIndex(x => x.Status);
        builder.HasOne(x => x.Asset).WithMany().HasForeignKey(x => x.AssetId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Comments).WithOne().HasForeignKey(x => x.WorkOrderId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Attachments).WithOne().HasForeignKey(x => x.WorkOrderId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.History).WithOne().HasForeignKey(x => x.WorkOrderId).OnDelete(DeleteBehavior.Cascade);
    }
}
