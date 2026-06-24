using AttendanceDomain.Attendance.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AttendanceDomain.Data.Configurations;

public class ShiftScheduleAssignmentConfiguration : IEntityTypeConfiguration<ShiftScheduleAssignment>
{
    public void Configure(EntityTypeBuilder<ShiftScheduleAssignment> builder)
    {
        builder.ToTable("ShiftScheduleAssignments");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasIndex(x => new { x.ScheduleId, x.EmployeeId, x.WorkDate })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
        builder.HasIndex(x => new { x.CompanyId, x.WorkDate });
        builder.HasIndex(x => x.ShiftId);
    }
}
