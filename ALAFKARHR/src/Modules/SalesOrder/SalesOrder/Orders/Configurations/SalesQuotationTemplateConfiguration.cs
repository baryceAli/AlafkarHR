using SalesOrder.Orders.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SalesOrder.Orders.Configurations;

public class SalesQuotationTemplateConfiguration : IEntityTypeConfiguration<SalesQuotationTemplate>
{
    public void Configure(EntityTypeBuilder<SalesQuotationTemplate> builder)
    {
        builder.HasMany(x => x.Lines)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.DownPaymentAmount).HasPrecision(18, 4);
        builder.Property(x => x.DownPaymentPercent).HasPrecision(9, 4);
    }
}

public class SalesQuotationTemplateLineConfiguration : IEntityTypeConfiguration<SalesQuotationTemplateLine>
{
    public void Configure(EntityTypeBuilder<SalesQuotationTemplateLine> builder)
    {
        builder.Property(x => x.ProductName).HasMaxLength(250);
        builder.Property(x => x.ProductNameEng).HasMaxLength(250);
        builder.Property(x => x.SkuCode).HasMaxLength(100);
        builder.Property(x => x.Quantity).HasPrecision(18, 4);
        builder.Property(x => x.UnitPrice).HasPrecision(18, 4);
        builder.Property(x => x.DiscountRate).HasPrecision(9, 4);
        builder.Property(x => x.TaxRate).HasPrecision(9, 4);
    }
}
