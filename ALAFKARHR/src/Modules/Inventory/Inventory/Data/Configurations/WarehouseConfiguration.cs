using Inventory.Warehouses.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Data.Configurations
{
    public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
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
            builder.Property(x => x.BranchId);
            builder.Property(x => x.WarehouseType)
                .HasConversion<string>()
                .HasMaxLength(40)
                .HasDefaultValue(WarehouseType.Commercial);
            builder.Property(x => x.ShortCode)
                .HasMaxLength(20);
            builder.Property(x => x.InboundFlow)
                .HasConversion<string>()
                .HasMaxLength(40)
                .HasDefaultValue(WarehouseOperationFlow.OneStep);
            builder.Property(x => x.OutboundFlow)
                .HasConversion<string>()
                .HasMaxLength(40)
                .HasDefaultValue(WarehouseOperationFlow.OneStep);
            builder.Property(x => x.DefaultSourceLocationId);
            builder.Property(x => x.DefaultDestinationLocationId);
            builder.Property(x => x.DefaultQualityLocationId);
            builder.Property(x => x.DefaultPackingLocationId);
            builder.Property(x => x.DefaultOutputLocationId);
            builder.Property(x => x.DefaultTransitLocationId);

            builder.HasMany(x => x.ResupplyFromLinks)
                .WithOne()
                .HasForeignKey(x => x.WarehouseId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(x => x.ResupplyFromLinks)
                .HasField("_resupplyFromLinks")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasIndex(x => new { x.CompanyId, x.BranchId });
            builder.HasIndex(x => new { x.CompanyId, x.ShortCode })
                .HasFilter("[ShortCode] IS NOT NULL");

            // Audit fields are provided by base types; allow nulls
            builder.Property<DateTime?>("CreatedAt");
            builder.Property<string?>("CreatedBy");
            builder.Property<DateTime?>("LastModified");
            builder.Property<string?>("LastModifiedBy");
            builder.Property<DateTime?>("DeletedAt");
            builder.Property<string?>("DeletedBy");
        }
    }

    public class WarehouseResupplyLinkConfiguration : IEntityTypeConfiguration<WarehouseResupplyLink>
    {
        public void Configure(EntityTypeBuilder<WarehouseResupplyLink> builder)
        {
            builder.ToTable("WarehouseResupplyLinks", "Inventory");
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => new { x.CompanyId, x.WarehouseId, x.SourceWarehouseId }).IsUnique();
            builder.HasIndex(x => new { x.CompanyId, x.SourceWarehouseId });
            builder.Property(x => x.CreatedBy).HasMaxLength(100);
            builder.Property(x => x.ModifiedBy).HasMaxLength(100);
            builder.Property(x => x.DeletedBy).HasMaxLength(100);
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
