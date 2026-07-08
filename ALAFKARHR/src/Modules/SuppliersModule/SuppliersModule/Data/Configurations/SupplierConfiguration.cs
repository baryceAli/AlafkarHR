using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SuppliersModule.Suppliers.Models;

namespace SuppliersModule.Data.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("Suppliers");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.CommercialName).HasMaxLength(200);
        builder.Property(x => x.SupplierCode).IsRequired().HasMaxLength(50);
        builder.Property(x => x.TaxNumber).HasMaxLength(50);
        builder.Property(x => x.FiscalPosition).HasMaxLength(100);
        builder.Property(x => x.VendorPaymentReference).HasMaxLength(100);
        builder.Property(x => x.Notes).HasMaxLength(2000);
        builder.Property(x => x.CompanyId).IsRequired();

        builder.Property(x => x.Status).HasConversion<int>().IsRequired();
        builder.Property(x => x.Type).HasConversion<int>().IsRequired();
        builder.Property(x => x.PaymentTerm).HasConversion<int>().IsRequired();

        builder.Property(x => x.CreditLimit).HasPrecision(18, 2);
        builder.Property(x => x.OpeningBalance).HasPrecision(18, 2);

        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);

        builder.HasIndex(x => new { x.CompanyId, x.SupplierCode }).IsUnique();
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.OwnsMany(x => x.Addresses, address =>
        {
            address.ToTable("SupplierAddresses");
            address.WithOwner().HasForeignKey("SupplierId");
            address.HasKey(x => x.Id);
            address.Property(x => x.Id).ValueGeneratedNever();

            address.Property(x => x.Title).IsRequired().HasMaxLength(100);
            address.Property(x => x.AddressLine1).IsRequired().HasMaxLength(300);
            address.Property(x => x.AddressLine2).HasMaxLength(300);
            address.Property(x => x.City).IsRequired().HasMaxLength(100);
            address.Property(x => x.State).IsRequired().HasMaxLength(100);
            address.Property(x => x.Country).IsRequired().HasMaxLength(100);
            address.Property(x => x.PostalCode).HasMaxLength(30);
            address.Property(x => x.Longitude).HasPrecision(18, 10);
            address.Property(x => x.Latitude).HasPrecision(18, 10);
            address.Property(x => x.IsDefaultBilling).IsRequired();
            address.Property(x => x.AddressType).HasConversion<int>().HasDefaultValue(PartnerAddressType.Contact).IsRequired();

            address.Property(x => x.CreatedBy).HasMaxLength(100);
            address.Property(x => x.ModifiedBy).HasMaxLength(100);
            address.Property(x => x.DeletedBy).HasMaxLength(100);
        });

        builder.OwnsMany(x => x.Contacts, contact =>
        {
            contact.ToTable("SupplierContacts");
            contact.WithOwner().HasForeignKey("SupplierId");
            contact.HasKey(x => x.Id);
            contact.Property(x => x.Id).ValueGeneratedNever();

            contact.Property(x => x.FullName).IsRequired().HasMaxLength(150);
            contact.Property(x => x.JobTitle).HasMaxLength(100);
            contact.Property(x => x.Email).HasMaxLength(200);
            contact.Property(x => x.PhoneNumber).HasMaxLength(50);
            contact.Property(x => x.IsPrimaryContact).IsRequired();
            contact.Property(x => x.ContactType).HasConversion<int>().HasDefaultValue(PartnerContactType.Contact).IsRequired();

            contact.Property(x => x.CreatedBy).HasMaxLength(100);
            contact.Property(x => x.ModifiedBy).HasMaxLength(100);
            contact.Property(x => x.DeletedBy).HasMaxLength(100);
        });

        builder.Navigation(x => x.Addresses).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(x => x.Contacts).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
