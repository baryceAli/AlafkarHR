namespace Procurement.Data.Configurations;

public class ProcurementDocumentConfiguration : IEntityTypeConfiguration<ProcurementDocument>
{
    public void Configure(EntityTypeBuilder<ProcurementDocument> builder)
    {
        builder.ToTable("ProcurementDocuments");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Kind).HasConversion<int>().IsRequired();
        builder.Property(x => x.Number).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Status).IsRequired().HasMaxLength(50);
        builder.Property(x => x.SupplierName).HasMaxLength(200);
        builder.Property(x => x.SourceDocumentNumber).HasMaxLength(50);
        builder.Property(x => x.SentBy).HasMaxLength(100);
        builder.Property(x => x.BillControlPolicy)
            .HasConversion<int>()
            .HasDefaultValue(PurchaseBillControlPolicy.OrderedQuantities)
            .IsRequired();
        builder.Property(x => x.ThreeWayMatchStatus)
            .HasConversion<int>()
            .HasDefaultValue(ThreeWayMatchStatus.NotRequired)
            .IsRequired();
        builder.Property(x => x.IsBillable)
            .HasDefaultValue(false)
            .IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(2000);
        builder.Property(x => x.Subtotal).HasPrecision(18, 2);
        builder.Property(x => x.TaxAmount).HasPrecision(18, 2);
        builder.Property(x => x.TotalAmount).HasPrecision(18, 2);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);
        builder.HasIndex(x => new { x.CompanyId, x.Kind, x.Number }).IsUnique();
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasDiscriminator<string>("DocumentType")
            .HasValue<PurchaseRequest>(nameof(PurchaseRequest))
            .HasValue<RequestForQuotation>(nameof(RequestForQuotation))
            .HasValue<SupplierQuotation>(nameof(SupplierQuotation))
            .HasValue<PurchaseOrder>(nameof(PurchaseOrder))
            .HasValue<GoodsReceipt>(nameof(GoodsReceipt))
            .HasValue<PurchaseReturn>(nameof(PurchaseReturn))
            .HasValue<SupplierInvoice>(nameof(SupplierInvoice));

        builder.OwnsMany(x => x.Lines, line =>
        {
            line.ToTable("ProcurementDocumentLines");
            line.WithOwner().HasForeignKey("ProcurementDocumentId");
            line.HasKey(x => x.Id);
            line.Property(x => x.Id).ValueGeneratedNever();
            line.Property(x => x.ProductName).IsRequired().HasMaxLength(200);
            line.Property(x => x.ProductNameEng).HasMaxLength(200);
            line.Property(x => x.SkuCode).HasMaxLength(100);
            line.Property(x => x.ReorderingRuleId);
            line.Property(x => x.BillControlPolicy).HasConversion<int>();
            line.Property(x => x.ThreeWayMatchStatus)
                .HasConversion<int>()
                .HasDefaultValue(ThreeWayMatchStatus.NotRequired)
                .IsRequired();
            line.Property(x => x.ReceivedQuantity).HasPrecision(18, 4);
            line.Property(x => x.BilledQuantity).HasPrecision(18, 4);
            line.Property(x => x.Quantity).HasPrecision(18, 4);
            line.Property(x => x.UnitCost).HasPrecision(18, 4);
            line.Property(x => x.DiscountRate).HasPrecision(18, 4);
            line.Property(x => x.TaxRate).HasPrecision(18, 4);
            line.Property(x => x.NetAmount).HasPrecision(18, 2);
            line.Property(x => x.TaxAmount).HasPrecision(18, 2);
            line.Property(x => x.TotalAmount).HasPrecision(18, 2);
            line.Property(x => x.Notes).HasMaxLength(1000);
            line.Property(x => x.CreatedBy).HasMaxLength(100);
            line.Property(x => x.ModifiedBy).HasMaxLength(100);
            line.Property(x => x.DeletedBy).HasMaxLength(100);
            line.HasIndex(x => x.ReorderingRuleId);
        });

        builder.Navigation(x => x.Lines).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
