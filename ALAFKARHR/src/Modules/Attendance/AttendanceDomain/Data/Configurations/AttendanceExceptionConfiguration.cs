using AttendanceDomain.Attendance.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AttendanceDomain.Data.Configurations;

public class AttendanceExceptionConfiguration : IEntityTypeConfiguration<AttendanceException>
{
    public void Configure(EntityTypeBuilder<AttendanceException> builder)
    {
        builder.ToTable("AttendanceExceptions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ExceptionType).HasConversion<int>().IsRequired();
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();
        builder.Property(x => x.Reason).IsRequired().HasMaxLength(1000);
        builder.Property(x => x.ManagerNote).HasMaxLength(1000);

        builder.HasIndex(x => x.EmployeeId);
        builder.HasIndex(x => x.SessionId);
        builder.HasIndex(x => new { x.Status, x.ExceptionType });
    }
}
