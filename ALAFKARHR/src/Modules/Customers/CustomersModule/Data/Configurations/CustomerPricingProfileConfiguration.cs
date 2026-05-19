namespace CustomersModule.Data.Configurations;

using CustomersModule.Customers.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


public class CustomerPricingProfileConfiguration
    : IEntityTypeConfiguration<CustomerPricingProfile>
{
    public void Configure(EntityTypeBuilder<CustomerPricingProfile> builder)
    {
        builder.ToTable("CustomerPricingProfiles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired();
            //.ValueGeneratedNever();

        builder.Property(x => x.CustomerId)
            .IsRequired();

        builder.Property(x => x.PriceListId)
            .IsRequired();

        builder.Property(x => x.DiscountPercentage)
            .HasPrecision(5, 2);

        builder.Property(x => x.AllowAdditionalDiscounts)
            .IsRequired();

        builder.Property(x => x.EffectiveFrom)
            .IsRequired();

        builder.Property(x => x.EffectiveTo);

        builder.Property(x=> x.CompanyId) .IsRequired();

        // Audit Fields
        builder.Property(x => x.CreatedBy)
            .HasMaxLength(100);

        builder.Property(x => x.ModifiedBy)
            .HasMaxLength(100);

        builder.Property(x => x.DeletedBy)
            .HasMaxLength(100);

        // Useful indexes
        builder.HasIndex(x => x.CustomerId);

        builder.HasIndex(x => new
        {
            x.CustomerId,
            x.PriceListId,
            x.EffectiveFrom
        });

        // Soft delete filter
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}