namespace DocumentManagement.Data;

public class DocumentManagementDbContext(DbContextOptions<DocumentManagementDbContext> options) : DbContext(options)
{
    public DbSet<DocumentItem> Documents => Set<DocumentItem>();
    public DbSet<DocumentVersion> DocumentVersions => Set<DocumentVersion>();
    public DbSet<DocumentCollaborator> DocumentCollaborators => Set<DocumentCollaborator>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("DocumentManagement");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
