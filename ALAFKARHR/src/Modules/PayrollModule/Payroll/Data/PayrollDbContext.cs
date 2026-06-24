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
    public DbSet<EmployeeLoan> EmployeeLoans => Set<EmployeeLoan>();
    public DbSet<EmployeeContract> EmployeeContracts => Set<EmployeeContract>();
    public DbSet<SalaryStructure> SalaryStructures => Set<SalaryStructure>();
    public DbSet<SalaryStructureLine> SalaryStructureLines => Set<SalaryStructureLine>();
    public DbSet<SalaryStructureAssignment> SalaryStructureAssignments => Set<SalaryStructureAssignment>();
    public DbSet<PayrollPeriod> PayrollPeriods => Set<PayrollPeriod>();
    public DbSet<PayrollEntry> PayrollEntries => Set<PayrollEntry>();
    public DbSet<Payslip> Payslips => Set<Payslip>();
    public DbSet<PayslipLine> PayslipLines => Set<PayslipLine>();
    public DbSet<PayrollInput> PayrollInputs => Set<PayrollInput>();
    public DbSet<SaudiPayrollInfo> SaudiPayrollInfos => Set<SaudiPayrollInfo>();
    public DbSet<WpsBatch> WpsBatches => Set<WpsBatch>();
    public DbSet<WpsBatchRow> WpsBatchRows => Set<WpsBatchRow>();
    public DbSet<EosProvisionSnapshot> EosProvisionSnapshots => Set<EosProvisionSnapshot>();
    public DbSet<PayrollImportedWorkEntry> PayrollImportedWorkEntries => Set<PayrollImportedWorkEntry>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("Payroll");
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }

}
