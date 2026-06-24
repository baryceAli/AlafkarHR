using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Training.Data.Configurations;

public class TrainingProgramConfiguration : IEntityTypeConfiguration<TrainingProgram>
{
    public void Configure(EntityTypeBuilder<TrainingProgram> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(250).IsRequired();
        builder.Property(x => x.Category).HasMaxLength(150);
        builder.Property(x => x.Provider).HasMaxLength(250);
        builder.Property(x => x.Objective).HasMaxLength(1000);
        builder.Property(x => x.Description).HasMaxLength(2000);
        builder.HasIndex(x => new { x.CompanyId, x.Name });
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class TrainingEventConfiguration : IEntityTypeConfiguration<TrainingEvent>
{
    public void Configure(EntityTypeBuilder<TrainingEvent> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).HasMaxLength(250).IsRequired();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(40);
        builder.HasIndex(x => new { x.CompanyId, x.ProgramId, x.Status, x.StartAt });
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class TrainingAttendeeConfiguration : IEntityTypeConfiguration<TrainingAttendee>
{
    public void Configure(EntityTypeBuilder<TrainingAttendee> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(40);
        builder.Property(x => x.Score).HasPrecision(18, 2);
        builder.Property(x => x.Feedback).HasMaxLength(2000);
        builder.Property(x => x.CertificateName).HasMaxLength(250);
        builder.Property(x => x.CertificateIssuer).HasMaxLength(250);
        builder.HasIndex(x => new { x.TrainingEventId, x.EmployeeId }).IsUnique().HasFilter("[IsDeleted] = 0");
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
