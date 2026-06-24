using System.Reflection;

namespace PerformanceManagement.Data;

public class PerformanceDbContext(DbContextOptions<PerformanceDbContext> options) : DbContext(options)
{
    public DbSet<PerformanceCycle> PerformanceCycles => Set<PerformanceCycle>();
    public DbSet<GoalDefinition> GoalDefinitions => Set<GoalDefinition>();
    public DbSet<Competency> Competencies => Set<Competency>();
    public DbSet<EmployeeGoal> EmployeeGoals => Set<EmployeeGoal>();
    public DbSet<EmployeeCompetencyScore> EmployeeCompetencyScores => Set<EmployeeCompetencyScore>();
    public DbSet<PerformanceEvaluation> PerformanceEvaluations => Set<PerformanceEvaluation>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("PerformanceManagement");
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
}
