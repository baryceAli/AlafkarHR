using Inventory.Warehouses.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Data.Configurations
{
    internal class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
    {
        public void Configure(EntityTypeBuilder<Warehouse> builder)
        {
            builder.ToTable("Warehouses", "Inventory");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.NameEng)
                .IsRequired();
            builder.Property(x => x.Location)
                .IsRequired();

            builder.Property(x => x.Address);

            builder.Property(x => x.Longitude);
            builder.Property(x => x.Latitude);

            // Audit fields are provided by base types; allow nulls
            builder.Property<DateTime?>("CreatedAt");
            builder.Property<string?>("CreatedBy");
            builder.Property<DateTime?>("LastModified");
            builder.Property<string?>("LastModifiedBy");
            builder.Property<DateTime?>("DeletedAt");
            builder.Property<string?>("DeletedBy");
        }
    }
}
