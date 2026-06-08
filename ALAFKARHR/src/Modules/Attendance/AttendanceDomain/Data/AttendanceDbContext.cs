using AttendanceDomain.Attendance.Models;
using System.Reflection;

namespace AttendanceDomain.Data;
//add-migration AuthInitial -Project AttendanceDomain -StartupProject Api -OutputDir Data/Migrations -Context AttendanceDbContext
//update-database -Project AttendanceDomain -StartupProject Api -Context AttendanceDbContext

public class AttendanceDbContext(DbContextOptions<AttendanceDbContext> options) : DbContext(options)
{
    public DbSet<AttendanceDay> AttendanceDays => Set<AttendanceDay>();
    public DbSet<AttendanceLog> AttendanceLogs => Set<AttendanceLog>();
    public DbSet<Shift> Shifts => Set<Shift>();
    public DbSet<EmployeeShift> EmployeeShifts => Set<EmployeeShift>();
    public DbSet<AttendanceSession> AttendanceSessions => Set<AttendanceSession>();
    public DbSet<AttendanceLocationPing> AttendanceLocationPings => Set<AttendanceLocationPing>();
    public DbSet<AttendanceCheckIn> AttendanceCheckIns => Set<AttendanceCheckIn>();
    public DbSet<AttendanceException> AttendanceExceptions => Set<AttendanceException>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("Attendance");
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
}
