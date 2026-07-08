using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalesOrder.Orders.Models;

namespace SalesOrder.Data.Configurations;

public class SalesOrderConfiguration : IEntityTypeConfiguration<Orders.Models.SalesOrder>
{
    public void Configure(EntityTypeBuilder<Orders.Models.SalesOrder> builder)
    {
        builder.Property(x => x.DownPaymentAmount).HasPrecision(18, 2);
        builder.Property(x => x.DownPaymentPercent).HasPrecision(18, 4);
        builder.Property(x => x.RequiresCustomerSignature).HasDefaultValue(false).IsRequired();
        builder.Property(x => x.RequiresOnlinePayment).HasDefaultValue(false).IsRequired();
        builder.Property(x => x.IsProForma).HasDefaultValue(false).IsRequired();
    }
}

public class SalesQuotationConfiguration : IEntityTypeConfiguration<SalesQuotation>
{
    public void Configure(EntityTypeBuilder<SalesQuotation> builder)
    {
        builder.Property(x => x.DownPaymentAmount).HasPrecision(18, 2);
        builder.Property(x => x.DownPaymentPercent).HasPrecision(18, 4);
        builder.Property(x => x.RequiresCustomerSignature).HasDefaultValue(false).IsRequired();
        builder.Property(x => x.RequiresOnlinePayment).HasDefaultValue(false).IsRequired();
        builder.Property(x => x.IsProForma).HasDefaultValue(false).IsRequired();
    }
}

public class SalesQuotationLineConfiguration : IEntityTypeConfiguration<SalesQuotationLine>
{
    public void Configure(EntityTypeBuilder<SalesQuotationLine> builder)
    {
        builder.Property(x => x.IsOptional).HasDefaultValue(false).IsRequired();
    }
}
