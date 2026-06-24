using EmployeeModule.Employees.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeModule.Data.Configurations;

public class HrLifecycleEventConfiguration : IEntityTypeConfiguration<HrLifecycleEvent>
{
    public void Configure(EntityTypeBuilder<HrLifecycleEvent> builder)
    {
        builder.ToTable("HrLifecycleEvents");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EventType).HasConversion<int>().IsRequired();
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();
        builder.Property(x => x.Reason).HasMaxLength(1000);
        builder.Property(x => x.Notes).HasMaxLength(2000);
        builder.Property(x => x.FromGrade).HasMaxLength(100);
        builder.Property(x => x.ToGrade).HasMaxLength(100);
        builder.Property(x => x.FromWorkLocation).HasMaxLength(250);
        builder.Property(x => x.ToWorkLocation).HasMaxLength(250);
        builder.HasIndex(x => new { x.CompanyId, x.EmployeeId, x.Status });
        builder.HasIndex(x => x.EffectiveDate);
    }
}

public class EmployeeEmergencyContactConfiguration : IEntityTypeConfiguration<EmployeeEmergencyContact>
{
    public void Configure(EntityTypeBuilder<EmployeeEmergencyContact> builder)
    {
        builder.ToTable("EmployeeEmergencyContacts");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Relationship).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Phone).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Email).HasMaxLength(200);
        builder.HasIndex(x => new { x.CompanyId, x.EmployeeId, x.IsPrimary });
    }
}

public class EmployeeDocumentLinkConfiguration : IEntityTypeConfiguration<EmployeeDocumentLink>
{
    public void Configure(EntityTypeBuilder<EmployeeDocumentLink> builder)
    {
        builder.ToTable("EmployeeDocumentLinks");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.DocumentType).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Title).IsRequired().HasMaxLength(250);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasIndex(x => new { x.CompanyId, x.EmployeeId, x.DocumentType });
        builder.HasIndex(x => x.DocumentId);
        builder.HasIndex(x => x.ExpiryDate);
    }
}

public class EmployeeSkillConfiguration : IEntityTypeConfiguration<EmployeeSkill>
{
    public void Configure(EntityTypeBuilder<EmployeeSkill> builder)
    {
        builder.ToTable("EmployeeSkills");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.SkillName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Category).HasMaxLength(100);
        builder.Property(x => x.Source).HasMaxLength(200);
        builder.HasIndex(x => new { x.CompanyId, x.EmployeeId, x.SkillName });
    }
}

public class EmployeeCertificationConfiguration : IEntityTypeConfiguration<EmployeeCertification>
{
    public void Configure(EntityTypeBuilder<EmployeeCertification> builder)
    {
        builder.ToTable("EmployeeCertifications");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Issuer).HasMaxLength(200);
        builder.HasIndex(x => new { x.CompanyId, x.EmployeeId, x.Name });
        builder.HasIndex(x => x.DocumentId);
        builder.HasIndex(x => x.ExpiresAt);
    }
}
