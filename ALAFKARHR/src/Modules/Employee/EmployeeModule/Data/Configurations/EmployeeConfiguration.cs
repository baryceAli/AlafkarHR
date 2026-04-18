
using EmployeeModule.Employees.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeModule.Data.Configurations;
public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");

        builder.HasKey(x => x.Id);

        // 🔐 Identity
        builder.Property(x => x.EmployeeNo)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Email)
            .HasMaxLength(150);

        builder.Property(x => x.Phone)
            .HasMaxLength(30);

        builder.Property(x => x.NationalId)
            .HasMaxLength(50);

        // 📊 Relationships
        
        builder.HasOne(x => x.Position)
            .WithMany()
            .HasForeignKey(x => x.PositionId);

        // 👤 Self Reference (Manager)
        //builder.HasOne(x => x.Manager)
        //    .WithMany()
        //    .HasForeignKey(x => x.ManagerId)
        //    .OnDelete(DeleteBehavior.Restrict);

        // 🔍 Indexes (VERY IMPORTANT)
        builder.HasIndex(x => x.EmployeeNo).IsUnique();
        builder.HasIndex(x => x.NationalId).IsUnique();
        builder.HasIndex(x => x.Email).IsUnique();
        builder.HasIndex(x => x.Phone).IsUnique();

        builder.HasIndex(x => new { x.CompanyId, x.EmployeeNo });

        builder.HasIndex(x => x.DepartmentId);
        builder.HasIndex(x => x.BranchId);
        builder.HasIndex(x => x.PositionId);
    }
}