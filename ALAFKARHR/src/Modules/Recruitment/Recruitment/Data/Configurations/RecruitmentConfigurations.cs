using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Recruitment.Data.Configurations;

public class StaffingPlanConfiguration : IEntityTypeConfiguration<StaffingPlan>
{
    public void Configure(EntityTypeBuilder<StaffingPlan> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasIndex(x => new { x.CompanyId, x.Year, x.DepartmentId, x.PositionId });
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class JobRequisitionConfiguration : IEntityTypeConfiguration<JobRequisition>
{
    public void Configure(EntityTypeBuilder<JobRequisition> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).HasMaxLength(250).IsRequired();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(40);
        builder.HasIndex(x => new { x.CompanyId, x.Status, x.RequestedAt });
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class ApplicantConfiguration : IEntityTypeConfiguration<Applicant>
{
    public void Configure(EntityTypeBuilder<Applicant> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FullName).HasMaxLength(250).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(250);
        builder.Property(x => x.Phone).HasMaxLength(50);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(40);
        builder.HasIndex(x => new { x.CompanyId, x.JobRequisitionId, x.Status });
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class InterviewFeedbackConfiguration : IEntityTypeConfiguration<InterviewFeedback>
{
    public void Configure(EntityTypeBuilder<InterviewFeedback> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Feedback).HasMaxLength(2000);
        builder.HasIndex(x => new { x.ApplicantId, x.InterviewAt });
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class JobOfferConfiguration : IEntityTypeConfiguration<JobOffer>
{
    public void Configure(EntityTypeBuilder<JobOffer> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ProposedSalary).HasPrecision(18, 2);
        builder.HasIndex(x => new { x.ApplicantId, x.OfferDate });
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
