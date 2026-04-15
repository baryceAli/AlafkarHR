
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Organization.Organizations.Models;

namespace Organization.Data.Configurations;
public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ToTable("Branches");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.NameEng)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Location)
            .HasMaxLength(300);

        builder.Property(x => x.Longitude)
            .HasPrecision(10, 6);

        builder.Property(x => x.Latitude)
            .HasPrecision(10, 6);

        // 🔗 Shadow FK (until you add OrganizationId property)
        //builder.Property<Guid>("CompanyId");



        // 🔍 Indexes
        builder.HasIndex(x => x.CompanyId);
        builder.HasIndex(x => x.Code);
        builder.HasIndex(x => new { x.CompanyId, x.Code }).IsUnique();
        // 🧾 Audit
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);
    }
}