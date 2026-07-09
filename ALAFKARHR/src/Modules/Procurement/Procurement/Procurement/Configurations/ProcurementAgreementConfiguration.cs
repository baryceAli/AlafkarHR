namespace Procurement.Procurement.Configurations;

public class ProcurementAgreementConfiguration : IEntityTypeConfiguration<ProcurementAgreement>
{
    public void Configure(EntityTypeBuilder<ProcurementAgreement> builder)
    {
        builder.HasMany(x => x.Lines)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.Reference).HasMaxLength(100);
        builder.Property(x => x.SupplierName).HasMaxLength(250);
    }
}

public class ProcurementAgreementLineConfiguration : IEntityTypeConfiguration<ProcurementAgreementLine>
{
    public void Configure(EntityTypeBuilder<ProcurementAgreementLine> builder)
    {
        builder.Property(x => x.ProductName).HasMaxLength(250);
        builder.Property(x => x.ProductNameEng).HasMaxLength(250);
        builder.Property(x => x.SkuCode).HasMaxLength(100);
        builder.Property(x => x.Quantity).HasPrecision(18, 4);
        builder.Property(x => x.UnitCost).HasPrecision(18, 4);
        builder.Property(x => x.DiscountRate).HasPrecision(9, 4);
        builder.Property(x => x.TaxRate).HasPrecision(9, 4);
    }
}
