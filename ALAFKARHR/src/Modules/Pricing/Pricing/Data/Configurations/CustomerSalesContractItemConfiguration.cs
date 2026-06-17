namespace Pricing.Data.Configurations;

public class CustomerSalesContractItemConfiguration : IEntityTypeConfiguration<CustomerSalesContractItem>
{
    public void Configure(EntityTypeBuilder<CustomerSalesContractItem> builder)
    {
        builder.ToTable("CustomerSalesContractItems");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.CustomerSalesContractId).IsRequired();
        builder.Property(x => x.ProductSkuId).IsRequired();
        builder.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");
        builder.Property(x => x.MinQuantity).HasColumnType("decimal(18,2)");
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);

        builder.HasIndex(x => new { x.CustomerSalesContractId, x.ProductSkuId, x.UnitId, x.MinQuantity });
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
