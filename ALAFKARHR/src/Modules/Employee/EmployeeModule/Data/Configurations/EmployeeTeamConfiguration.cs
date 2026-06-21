using EmployeeModule.Employees.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeModule.Data.Configurations;

public class EmployeeTeamConfiguration : IEntityTypeConfiguration<EmployeeTeam>
{
    public void Configure(EntityTypeBuilder<EmployeeTeam> builder)
    {
        builder.ToTable("EmployeeTeams");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.NameEng).HasMaxLength(200);
        builder.Property(x => x.Category).HasConversion<int>().IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.Property(x => x.IsActive).HasDefaultValue(true);

        builder.HasMany(x => x.Members)
            .WithOne()
            .HasForeignKey(x => x.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.CompanyId, x.Category, x.IsActive });
        builder.HasIndex(x => x.CreatedForProjectId);
    }
}

public class EmployeeTeamMemberConfiguration : IEntityTypeConfiguration<EmployeeTeamMember>
{
    public void Configure(EntityTypeBuilder<EmployeeTeamMember> builder)
    {
        builder.ToTable("EmployeeTeamMembers");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.EmployeeName).IsRequired().HasMaxLength(250);
        builder.Property(x => x.EmployeeNameEng).HasMaxLength(250);
        builder.Property(x => x.EmployeeNo).HasMaxLength(50);

        builder.HasIndex(x => new { x.TeamId, x.EmployeeId }).IsUnique();
        builder.HasIndex(x => x.EmployeeId);
    }
}
