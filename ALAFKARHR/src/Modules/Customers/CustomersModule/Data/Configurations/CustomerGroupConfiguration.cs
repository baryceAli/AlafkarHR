namespace CustomersModule.Data.Configurations;


using CustomersModule.Customers.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


public class CustomerGroupConfiguration : IEntityTypeConfiguration<CustomerGroup>
{
    public void Configure(EntityTypeBuilder<CustomerGroup> builder)
    {
        builder.ToTable("CustomerGroups");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.DefaultDiscountPercentage)
            .HasPrecision(5, 2);

        builder.Property(x => x.DefaultPriceListId);

        builder.Property(x => x.CompanyId).IsRequired();

        // Audit Fields
        builder.Property(x => x.CreatedBy)
            .HasMaxLength(100);

        builder.Property(x => x.ModifiedBy)
            .HasMaxLength(100);

        builder.Property(x => x.DeletedBy)
            .HasMaxLength(100);

        // Soft Delete Filter
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}