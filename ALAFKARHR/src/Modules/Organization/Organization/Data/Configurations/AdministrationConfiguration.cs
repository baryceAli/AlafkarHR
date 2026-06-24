
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Organization.Organizations.Models;

namespace Organization.Data.Configurations;

public class AdministrationConfiguration : IEntityTypeConfiguration<Administration>
{
    public void Configure(EntityTypeBuilder<Administration> builder)
    {
        builder.ToTable("Administrations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.NameEng)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.BranchId).IsRequired(false);
        builder.Property(x => x.CompanyId).IsRequired();
        builder.Property(x => x.ParentAdministrationId).IsRequired(false);
        builder.Property(x => x.IsHigherManagement)
            .IsRequired()
            .HasDefaultValue(false);



        builder.HasOne(x => x.Branch)
    .WithMany(x => x.Administrations)
    .HasForeignKey(x => x.BranchId)
    .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Company>()
    .WithMany()
    .HasForeignKey(x => x.CompanyId)
    .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ParentAdministration)
            .WithMany(x => x.ChildAdministrations)
            .HasForeignKey(x => x.ParentAdministrationId)
            .OnDelete(DeleteBehavior.Restrict);

        // 🔍 Indexes
        builder.HasIndex(x => x.CompanyId);
        builder.HasIndex(x => x.BranchId);
        builder.HasIndex(x => x.ParentAdministrationId);
        builder.HasIndex(x => new { x.CompanyId, x.BranchId });

        // 🧾 Audit
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);
    }
}
