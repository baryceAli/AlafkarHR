namespace DocumentManagement.Data;

public class DocumentManagementDbContext(DbContextOptions<DocumentManagementDbContext> options) : DbContext(options)
{
    //add-migration EmployeeInitial -Project DocumentManagement -StartupProject Api -OutputDir Data/Migrations -Context DocumentManagementDbContext
    //update-database -Project DocumentManagement -StartupProject Api -Context DocumentManagementDbContext

    public DbSet<DocumentItem> Documents => Set<DocumentItem>();
    public DbSet<DocumentVersion> DocumentVersions => Set<DocumentVersion>();
    public DbSet<DocumentCollaborator> DocumentCollaborators => Set<DocumentCollaborator>();
    public DbSet<DocumentUploadPolicy> DocumentUploadPolicies => Set<DocumentUploadPolicy>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("DocumentManagement");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
