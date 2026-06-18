using System.Reflection;

namespace Contracts.Contracts.Data;

public class ContractsDbContext(DbContextOptions<ContractsDbContext> options) : DbContext(options)
{
    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<ContractTemplate> ContractTemplates => Set<ContractTemplate>();
    public DbSet<ContractRenewal> ContractRenewals => Set<ContractRenewal>();
    public DbSet<ContractAttachment> ContractAttachments => Set<ContractAttachment>();
    public DbSet<ContractStatusHistory> ContractStatusHistory => Set<ContractStatusHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Contracts");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
