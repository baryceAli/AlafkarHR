using AttendanceDomain.Attendance.Models;
using System.Reflection;

namespace AttendanceDomain.Data;
//add-migration AttendanceInitial -Project AttendanceDomain -StartupProject Api -OutputDir Data/Migrations -Context AttendanceDbContext
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
    public DbSet<LateCheckInRequest> LateCheckInRequests => Set<LateCheckInRequest>();
    public DbSet<AttendanceConfiguration> AttendanceConfigurations => Set<AttendanceConfiguration>();
    public DbSet<AttendanceHoliday> AttendanceHolidays => Set<AttendanceHoliday>();
    public DbSet<MidDayPermissionRequest> MidDayPermissionRequests => Set<MidDayPermissionRequest>();
    public DbSet<LegacyAttendanceEmergencyLeaveRequest> LegacyEmergencyLeaveRequests => Set<LegacyAttendanceEmergencyLeaveRequest>();
    public DbSet<LegacyAttendanceEmployeeLeaveBalance> LegacyEmployeeLeaveBalances => Set<LegacyAttendanceEmployeeLeaveBalance>();
    public DbSet<ShiftSchedule> ShiftSchedules => Set<ShiftSchedule>();
    public DbSet<ShiftScheduleAssignment> ShiftScheduleAssignments => Set<ShiftScheduleAssignment>();
    public DbSet<ShiftSwapRequest> ShiftSwapRequests => Set<ShiftSwapRequest>();
    public DbSet<AttendanceCorrection> AttendanceCorrections => Set<AttendanceCorrection>();
    public DbSet<BiometricImportBatch> BiometricImportBatches => Set<BiometricImportBatch>();
    public DbSet<BiometricImportRow> BiometricImportRows => Set<BiometricImportRow>();
    public DbSet<AttendanceWorkEntry> AttendanceWorkEntries => Set<AttendanceWorkEntry>();
    public DbSet<AttendanceRosterSubstituteConfiguration> AttendanceRosterSubstituteConfigurations => Set<AttendanceRosterSubstituteConfiguration>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("Attendance");
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
}
