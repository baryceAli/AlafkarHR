using LeaveManagement.Data.Configurations;
using LeaveManagement.Leave.Models;

namespace LeaveManagement.Data;

public class LeaveDbContext(DbContextOptions<LeaveDbContext> options) : DbContext(options)
{
    public DbSet<EmergencyLeaveRequest> EmergencyLeaveRequests => Set<EmergencyLeaveRequest>();
    public DbSet<EmployeeLeaveBalance> EmployeeLeaveBalances => Set<EmployeeLeaveBalance>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("Leave");
        builder.ApplyConfiguration(new EmergencyLeaveRequestConfiguration());
        builder.ApplyConfiguration(new EmployeeLeaveBalanceConfiguration());
        base.OnModelCreating(builder);
    }
}
