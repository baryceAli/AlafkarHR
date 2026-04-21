using Microsoft.EntityFrameworkCore;
using Payroll.Salaries.Models;
using System.Reflection;

namespace Payroll.Data;

public class PayrollDbContext:DbContext
{
    public PayrollDbContext(DbContextOptions<PayrollDbContext> options):base(options){}


    //public DbSet<SalaryStructure> SalaryStructures => Set<SalaryStructure>();
    public DbSet<Contract> SalaryComponents => Set<Contract>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("Payroll");
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }

}
