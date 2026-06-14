using AttendanceDomain.Attendance.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AttendanceDomain.Data.Configurations;

public class AttendanceHolidayConfiguration : IEntityTypeConfiguration<AttendanceHoliday>
{
    public void Configure(EntityTypeBuilder<AttendanceHoliday> builder)
    {
        builder.ToTable("AttendanceHolidays");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.HolidayType).HasConversion<int>();
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.HasIndex(x => new { x.CompanyId, x.StartDate, x.EndDate });
        builder.HasIndex(x => new { x.CompanyId, x.IsActive });
    }
}
