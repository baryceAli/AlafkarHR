using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Organization.Organizations.Models;

namespace Organization.Data.Configurations;

public class UserBranchRoleAssignmentConfiguration : IEntityTypeConfiguration<UserBranchRoleAssignment>
{
    public void Configure(EntityTypeBuilder<UserBranchRoleAssignment> builder)
    {
        builder.ToTable("UserBranchRoleAssignments");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TemplateKey).IsRequired().HasMaxLength(100);
        builder.HasIndex(x => new { x.CompanyId, x.UserId, x.BranchId, x.TemplateKey })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
        builder.HasIndex(x => new { x.CompanyId, x.UserId });
        builder.HasIndex(x => x.BranchId);
        builder.HasOne<Company>()
            .WithMany()
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Branch>()
            .WithMany()
            .HasForeignKey(x => x.BranchId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);
    }
}
