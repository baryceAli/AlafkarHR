using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orders.Orders.Models;

namespace Orders.Data.Configurations;

public class OrderIntakeLineConfiguration : IEntityTypeConfiguration<OrderIntakeLine>
{
    public void Configure(EntityTypeBuilder<OrderIntakeLine> builder)
    {
        builder.ToTable("OrderIntakeLines");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.ProductName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.ProductNameEng).HasMaxLength(200);
        builder.Property(x => x.SkuCode).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Quantity).HasPrecision(18, 4);
        builder.Property(x => x.RequestedUnitPrice).HasPrecision(18, 2);
        builder.Property(x => x.RequestedDiscountRate).HasPrecision(5, 2);
        builder.Property(x => x.RequestedTaxRate).HasPrecision(5, 2);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);
        builder.HasIndex(x => x.ProductSkuId);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
