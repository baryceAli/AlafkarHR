
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Organization.Organizations.Models;
namespace Organization.Data.Configurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Companies");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.NameEng)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Logo)
            .HasMaxLength(500);

        builder.Property(x => x.HqLocation)
            .HasMaxLength(300);

        builder.Property(x => x.HqLongitude)
            .HasPrecision(10, 6);

        builder.Property(x => x.HqLatitude)
            .HasPrecision(10, 6);

        builder.Property(x => x.VatNo)
            .HasMaxLength(50);

        // 🔗 One-to-Many (Organization -> Branches)
        builder.HasMany(x => x.Branches)
            .WithOne() // no navigation back yet
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        // 🔍 Indexes
        builder.HasIndex(x => x.Name);
        builder.HasIndex(x => x.VatNo).IsUnique();
        builder.HasIndex(x => x.Code).IsUnique();
        
        // 🧾 Audit fields
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);
    }
}