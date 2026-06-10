using Microsoft.EntityFrameworkCore;
using Payroll.Salaries.Models;
using System.Reflection;

namespace Payroll.Data;

public class PayrollDbContext:DbContext
{
    public PayrollDbContext(DbContextOptions<PayrollDbContext> options):base(options){}

    //add-migration PayrollInitial -Project Payroll -StartupProject Api -OutputDir Data/Migrations -Context PayrollDbContext
    //update-database -Project Payroll -StartupProject Api -Context PayrollDbContext
    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<ContractItem> ContractItems => Set<ContractItem>();
    public DbSet<SalaryRun> SalaryRuns => Set<SalaryRun>();
    public DbSet<SalaryRunItem> SalaryRunItems => Set<SalaryRunItem>();
    public DbSet<Component> Components => Set<Component>();
    public DbSet<EmployeeAllowance> EmployeeAllowances => Set<EmployeeAllowance>();
    public DbSet<EmployeeDeduction> EmployeeDeductions => Set<EmployeeDeduction>();
    public DbSet<EmployeeContract> EmployeeContracts => Set<EmployeeContract>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("Payroll");
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }

}
