using Cart.Carts.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cart.Data.Configurations;

public class ShoppingCartLineConfiguration : IEntityTypeConfiguration<ShoppingCartLine>
{
    public void Configure(EntityTypeBuilder<ShoppingCartLine> builder)
    {
        builder.ToTable("CartLines");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.ProductName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.ProductNameEng).HasMaxLength(200);
        builder.Property(x => x.SkuCode).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Quantity).HasPrecision(18, 4);
        builder.Property(x => x.UnitPrice).HasPrecision(18, 2);
        builder.Property(x => x.DiscountRate).HasPrecision(5, 2);
        builder.Property(x => x.TaxRate).HasPrecision(5, 2);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);
        builder.HasIndex(x => x.ProductSkuId);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
