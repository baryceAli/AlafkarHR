using EmployeeModule.Employees.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace EmployeeModule.Data;

public class EmployeeDbContext:DbContext
{
    //add-migration EmployeeInitial -Project EmployeeModule -StartupProject Api -OutputDir Data/Migrations -Context EmployeeDbContext
    //update-database -Project EmployeeModule -StartupProject Api -Context EmployeeDbContext
    public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options):base(options)
    {
        
    }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Position> Positions => Set<Position>();
    public DbSet<AcademicInstitution> AcademicInstitutions => Set<AcademicInstitution>();
    public DbSet<Specialization> Specializations => Set<Specialization>();
    public DbSet<EmployeeTeam> EmployeeTeams => Set<EmployeeTeam>();
    public DbSet<EmployeeTeamMember> EmployeeTeamMembers => Set<EmployeeTeamMember>();
    public DbSet<HrLifecycleEvent> HrLifecycleEvents => Set<HrLifecycleEvent>();
    public DbSet<EmployeeEmergencyContact> EmployeeEmergencyContacts => Set<EmployeeEmergencyContact>();
    public DbSet<EmployeeDocumentLink> EmployeeDocumentLinks => Set<EmployeeDocumentLink>();
    public DbSet<EmployeeSkill> EmployeeSkills => Set<EmployeeSkill>();
    public DbSet<EmployeeCertification> EmployeeCertifications => Set<EmployeeCertification>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("Employee");
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }
}
