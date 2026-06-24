namespace Recruitment.Data;

public class RecruitmentDbContext(DbContextOptions<RecruitmentDbContext> options) : DbContext(options)
{
    public DbSet<StaffingPlan> StaffingPlans => Set<StaffingPlan>();
    public DbSet<JobRequisition> JobRequisitions => Set<JobRequisition>();
    public DbSet<Applicant> Applicants => Set<Applicant>();
    public DbSet<InterviewFeedback> InterviewFeedbacks => Set<InterviewFeedback>();
    public DbSet<JobOffer> JobOffers => Set<JobOffer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Recruitment");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RecruitmentDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
