using GeneralSettings.GeneralSettings.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GeneralSettings.Data;
// add-migration SettingsInitial -Project GeneralSettings -StartupProject Api -OutputDir Data/Migrations -Context GeneralSettingsDbContext
// update-database -Project GeneralSettings -StartupProject Api -Context GeneralSettingsDbContext

public class GeneralSettingsDbContext:DbContext
{
    public GeneralSettingsDbContext(DbContextOptions<GeneralSettingsDbContext> options):base(options)
    {
        
    }

    public DbSet<Currency> Currencies => Set<Currency>();
    public DbSet<CompanySetting> CompanySettings => Set<CompanySetting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("GeneralSettings");

        // 🔥 Apply all IEntityTypeConfiguration<>
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // 🔥 Global Conventions (optional but recommended)
        //ApplyGlobalConfigurations(modelBuilder);

        base.OnModelCreating(modelBuilder);

    }
}
