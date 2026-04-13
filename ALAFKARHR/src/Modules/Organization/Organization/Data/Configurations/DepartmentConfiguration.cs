
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Organization.Organizations.Models;

namespace Organization.Data.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Departments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.NameEng)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.AdministraitonId)
            .IsRequired();

        builder.HasOne(x => x.Administration)
    .WithMany(x => x.Departments)
    .HasForeignKey(x => x.AdministraitonId)
    .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ParentDepartment)
    .WithMany()
    .HasForeignKey(x => x.ParentDepartmentId)
    .OnDelete(DeleteBehavior.Restrict);
        // 🔍 Indexes

        builder.HasIndex(x => x.AdministraitonId);
        builder.HasIndex(x => x.CompanyId);
        builder.HasIndex(x => new { x.CompanyId, x.Code }).IsUnique();

        // 🧾 Audit
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);
    }
}