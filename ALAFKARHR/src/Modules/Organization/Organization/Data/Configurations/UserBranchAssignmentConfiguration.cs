using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Organization.Organizations.Models;

namespace Organization.Data.Configurations;

public class UserBranchAssignmentConfiguration : IEntityTypeConfiguration<UserBranchAssignment>
{
    public void Configure(EntityTypeBuilder<UserBranchAssignment> builder)
    {
        builder.ToTable("UserBranchAssignments");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.CompanyId, x.UserId, x.BranchId })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
        builder.HasIndex(x => new { x.CompanyId, x.UserId, x.IsDefault });
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
