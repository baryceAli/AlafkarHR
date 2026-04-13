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

        builder.Property(x => x.FinalScore).HasPrecision(5, 2);

        builder.HasIndex(x => new { x.EmployeeId, x.PerformanceCycleId })
            .IsUnique();

        builder.HasIndex(x => x.CompanyId);
    }
}