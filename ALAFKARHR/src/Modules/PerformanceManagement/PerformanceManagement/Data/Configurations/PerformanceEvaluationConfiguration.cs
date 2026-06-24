using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PerformanceManagement.Performances;

namespace PerformanceManagement.Data.Configurations;

public class PerformanceEvaluationConfiguration : IEntityTypeConfiguration<PerformanceEvaluation>
{
    public void Configure(EntityTypeBuilder<PerformanceEvaluation> builder)
    {
        builder.ToTable("PerformanceEvaluations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.KpiScore).HasPrecision(8, 2);
        builder.Property(x => x.CompetencyScore).HasPrecision(8, 2);
        builder.Property(x => x.FinalScore).HasPrecision(5, 2);
        builder.Property(x => x.Rating).HasConversion<string>().HasMaxLength(50);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(x => x.ManagerComment).HasMaxLength(1000);
        builder.Property(x => x.EmployeeComment).HasMaxLength(1000);

        builder.HasIndex(x => new { x.EmployeeId, x.PerformanceCycleId })
            .IsUnique();

        builder.HasIndex(x => x.CompanyId);
    }
}

public class PerformanceCycleConfiguration : IEntityTypeConfiguration<PerformanceCycle>
{
    public void Configure(EntityTypeBuilder<PerformanceCycle> builder)
    {
        builder.ToTable("PerformanceCycles");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.HasIndex(x => new { x.CompanyId, x.Name }).IsUnique().HasFilter("[IsDeleted] = 0");
        builder.HasIndex(x => new { x.CompanyId, x.IsActive, x.IsClosed, x.IsCancelled });
    }
}

public class GoalDefinitionConfiguration : IEntityTypeConfiguration<GoalDefinition>
{
    public void Configure(EntityTypeBuilder<GoalDefinition> builder)
    {
        builder.ToTable("GoalDefinitions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Code).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Weight).HasPrecision(8, 2);
        builder.HasIndex(x => new { x.CompanyId, x.Code }).IsUnique().HasFilter("[IsDeleted] = 0");
    }
}

public class CompetencyConfiguration : IEntityTypeConfiguration<Competency>
{
    public void Configure(EntityTypeBuilder<Competency> builder)
    {
        builder.ToTable("Competencies");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Weight).HasPrecision(8, 2);
        builder.HasIndex(x => new { x.CompanyId, x.Name }).IsUnique().HasFilter("[IsDeleted] = 0");
    }
}

public class EmployeeGoalConfiguration : IEntityTypeConfiguration<EmployeeGoal>
{
    public void Configure(EntityTypeBuilder<EmployeeGoal> builder)
    {
        builder.ToTable("EmployeeGoals");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TargetValue).HasPrecision(18, 2);
        builder.Property(x => x.AchievedValue).HasPrecision(18, 2);
        builder.Property(x => x.Weight).HasPrecision(8, 2);
        builder.HasIndex(x => new { x.EmployeeId, x.PerformanceCycleId, x.GoalDefinitionId }).IsUnique();
    }
}

public class EmployeeCompetencyScoreConfiguration : IEntityTypeConfiguration<EmployeeCompetencyScore>
{
    public void Configure(EntityTypeBuilder<EmployeeCompetencyScore> builder)
    {
        builder.ToTable("EmployeeCompetencyScores");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Score).HasPrecision(8, 2);
        builder.Property(x => x.Weight).HasPrecision(8, 2);
        builder.HasIndex(x => new { x.EmployeeId, x.PerformanceCycleId, x.CompetencyId }).IsUnique();
    }
}
