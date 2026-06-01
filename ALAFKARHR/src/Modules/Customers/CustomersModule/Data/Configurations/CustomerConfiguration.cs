namespace CustomersModule.Data.Configurations;

using CustomersModule.Customers.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.CommercialName)
            .HasMaxLength(200);

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Type)
            .HasConversion<int>()
            .IsRequired();

        //builder.Property(x => x.PaymentTerm)
        //    .HasConversion<int>()
        //    .IsRequired();

        builder.Property(x => x.CreditLimit)
            .HasPrecision(18, 2);

        builder.Property(x => x.Notes)
            .HasMaxLength(2000);

        builder.Property(x => x.IsTaxExempt)
            .IsRequired();

        builder.Property(x=> x.CompanyId) .IsRequired();

        // Audit Fields
        builder.Property(x => x.CreatedBy)
            .HasMaxLength(100);

        builder.Property(x => x.ModifiedBy)
            .HasMaxLength(100);

        builder.Property(x => x.DeletedBy)
            .HasMaxLength(100);

        // Query Filter
        builder.HasQueryFilter(x => !x.IsDeleted);

        // =========================
        // Addresses
        // =========================
        builder.OwnsMany(x => x.Addresses, address =>
        {
            address.ToTable("CustomerAddresses");

            address.WithOwner()
                .HasForeignKey("CustomerId");

            address.HasKey(x => x.Id);

            address.Property(x => x.Id)
                .ValueGeneratedNever();

            address.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(100);

            address.Property(x => x.AddressLine1)
                .IsRequired()
                .HasMaxLength(300);

            address.Property(x => x.AddressLine2)
                .HasMaxLength(300);

            address.Property(x => x.City)
                .IsRequired()
                .HasMaxLength(100);

            address.Property(x => x.State)
                .IsRequired()
                .HasMaxLength(100);

            address.Property(x => x.Country)
                .IsRequired()
                .HasMaxLength(100);

            address.Property(x => x.PostalCode)
                .HasMaxLength(30);

            address.Property(x => x.Longitude)
                .HasPrecision(18, 10);

            address.Property(x => x.Latitude)
                .HasPrecision(18, 10);

            address.Property(x => x.IsDefaultShipping)
                .IsRequired();

            // Audit
            address.Property(x => x.CreatedBy)
                .HasMaxLength(100);

            address.Property(x => x.ModifiedBy)
                .HasMaxLength(100);

            address.Property(x => x.DeletedBy)
                .HasMaxLength(100);

            //address.HasQueryFilter(x => !x.IsDeleted);
        });

        // =========================
        // Contacts
        // =========================
        builder.OwnsMany(x => x.Contacts, contact =>
        {
            contact.ToTable("CustomerContacts");

            contact.WithOwner()
                .HasForeignKey("CustomerId");

            contact.HasKey(x => x.Id);

            contact.Property(x => x.Id)
                .ValueGeneratedNever();

            contact.Property(x => x.FullName)
                .IsRequired()
                .HasMaxLength(150);

            contact.Property(x => x.JobTitle)
                .HasMaxLength(100);

            contact.Property(x => x.Email)
                .HasMaxLength(200);

            contact.Property(x => x.PhoneNumber)
                .HasMaxLength(50);

            contact.Property(x => x.IsPrimaryContact)
                .IsRequired();
            
            // Audit
            contact.Property(x => x.CreatedBy)
                .HasMaxLength(100);

            contact.Property(x => x.ModifiedBy)
                .HasMaxLength(100);

            contact.Property(x => x.DeletedBy)
                .HasMaxLength(100);

            //contact.HasQueryFilter(x => !x.IsDeleted);
        });

        builder.Navigation(x => x.Addresses)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(x => x.Contacts)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
