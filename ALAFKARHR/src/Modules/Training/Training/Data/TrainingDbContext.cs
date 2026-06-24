namespace Training.Data;

public class TrainingDbContext(DbContextOptions<TrainingDbContext> options) : DbContext(options)
{
    public DbSet<TrainingProgram> TrainingPrograms => Set<TrainingProgram>();
    public DbSet<TrainingEvent> TrainingEvents => Set<TrainingEvent>();
    public DbSet<TrainingAttendee> TrainingAttendees => Set<TrainingAttendee>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Training");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TrainingDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
