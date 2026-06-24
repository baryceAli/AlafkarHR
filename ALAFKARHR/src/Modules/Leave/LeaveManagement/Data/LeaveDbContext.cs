using LeaveManagement.Data.Configurations;
using LeaveManagement.Leave.Models;

namespace LeaveManagement.Data;

public class LeaveDbContext(DbContextOptions<LeaveDbContext> options) : DbContext(options)
{
    public DbSet<EmergencyLeaveRequest> EmergencyLeaveRequests => Set<EmergencyLeaveRequest>();
    public DbSet<EmployeeLeaveBalance> EmployeeLeaveBalances => Set<EmployeeLeaveBalance>();
    public DbSet<LeaveType> LeaveTypes => Set<LeaveType>();
    public DbSet<LeavePeriod> LeavePeriods => Set<LeavePeriod>();
    public DbSet<LeavePolicy> LeavePolicies => Set<LeavePolicy>();
    public DbSet<LeavePolicyLine> LeavePolicyLines => Set<LeavePolicyLine>();
    public DbSet<LeavePolicyAssignment> LeavePolicyAssignments => Set<LeavePolicyAssignment>();
    public DbSet<LeaveApplication> LeaveApplications => Set<LeaveApplication>();
    public DbSet<LeaveLedgerEntry> LeaveLedgerEntries => Set<LeaveLedgerEntry>();
    public DbSet<LeaveEncashmentRequest> LeaveEncashmentRequests => Set<LeaveEncashmentRequest>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("Leave");
        builder.ApplyConfiguration(new EmergencyLeaveRequestConfiguration());
        builder.ApplyConfiguration(new EmployeeLeaveBalanceConfiguration());
        builder.ApplyConfiguration(new LeaveTypeConfiguration());
        builder.ApplyConfiguration(new LeavePeriodConfiguration());
        builder.ApplyConfiguration(new LeavePolicyConfiguration());
        builder.ApplyConfiguration(new LeavePolicyLineConfiguration());
        builder.ApplyConfiguration(new LeavePolicyAssignmentConfiguration());
        builder.ApplyConfiguration(new LeaveApplicationConfiguration());
        builder.ApplyConfiguration(new LeaveLedgerEntryConfiguration());
        builder.ApplyConfiguration(new LeaveEncashmentRequestConfiguration());
        base.OnModelCreating(builder);
    }
}
